namespace Flame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Flame.Content
open Flame.Graphics
open Flame.Input

type GameSettings =
    { ScreenWidth: float32<pixel>
      ScreenHeight: float32<pixel> }

type GameApi = 
    { LoadFont: string -> Font 
      LoadTexture: string -> Texture }

type GameCommand<'TState> = 
    | Continue of state: 'TState
    | Exit

type MouseButton = 
    | Left
    | Middle
    | Right

type MouseEvent = 
    | Moved of position: Vector<pixel>
    | Button of button: MouseButton * state: MouseButtonState * position: Vector<pixel>

type GameEvent =
    | Mouse of event: MouseEvent

type Game<'TState> (
                    settings: GameSettings, 
                    state: 'TState, 
                    update: 'TState -> GameEvent list -> GameApi -> GameSettings -> float32<second> -> GameCommand<'TState>,
                    draw: 'TState -> float32<second> -> Graphics option) as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new GraphicsDeviceManager(this)
    let api = 
            {
                LoadFont    = (fun name -> this.Content.Load<SpriteFont>(name) |> Font)
                LoadTexture = (fun name -> this.Content.Load<Texture2D>(name)  |> Texture)
            }

    let mutable state = state
    let mutable mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState()
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let delta (gameTime: GameTime) = (float32 gameTime.ElapsedGameTime.TotalSeconds * 1.0f<second>)

    let handleMouseInput (state: Microsoft.Xna.Framework.Input.MouseState) (state': Microsoft.Xna.Framework.Input.MouseState) =
        seq {
            yield if state'.Position   <> state.Position   then state'.Position |> Utils.pointToPixelVector |> MouseEvent.Moved |> Some else None
            yield if state'.LeftButton <> state.LeftButton then MouseEvent.Button(MouseButton.Left, state'.LeftButton |> MouseInput.toButtonState,  Utils.pointToPixelVector state'.Position) |> Some else None
        } |> Seq.filter Option.isSome |> Seq.map Option.get |> Seq.map GameEvent.Mouse |> Seq.toList

    override _.LoadContent() =
        this.Content.RootDirectory <- "Content"
       
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

    override this.Initialize () =

        base.Initialize()

        graphics.PreferredBackBufferWidth  <- settings.ScreenWidth  |> int
        graphics.PreferredBackBufferHeight <- settings.ScreenHeight |> int
        
        this.Window.AllowUserResizing <- true

        graphics.ApplyChanges();

        base.IsMouseVisible <- true

    override _.Update (gameTime: GameTime) =

        match this.IsActive with 
        | true -> 
            let mouseState' = Mouse.GetState()
            let events = handleMouseInput mouseState mouseState'
            mouseState <- mouseState'

            match update state events api settings (delta gameTime) with
            | Continue state' -> state <- state'
            | Exit -> this.Exit()
        | false -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: GameTime) =
        
        match draw state (delta gameTime) with
        | Some g -> 
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue)
            Graphics.draw spriteBatch g
        | _ -> ()

module Game = 
    let run settings (state: 'TState) (update: 'TState -> GameEvent list -> GameApi -> GameSettings -> float32<second> -> GameCommand<'TState>) (draw: 'TState -> float32<second> -> Graphics option) = 
        let game = new Game<'TState>(settings, state, update, draw)
        game.Run()