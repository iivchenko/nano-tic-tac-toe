namespace Flame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Flame.Content
open Flame.Graphics
open Flame.Input

type SettingsCommand =
    | UpdateScreenSize of width: int * height: int

type ContentCommand = 
    | LoadFont of path: string
    | LoadTexture of path: string

type SettingsEvent =
    | ScreenSizeUpdated of width: int * height: int

type ContentEvent = 
    | FontLoaded of path: string * sprite: Font
    | TextureLoaded of path: string * texture: Texture

type GameCommand =
    | SettingsCommand of command: SettingsCommand
    | ContentCommand  of command: ContentCommand
    | ExitCommand

type MouseButton = 
    | Left
    | Middle
    | Right

type MouseEvent = 
    | Moved of position: Vector<pixel>
    | Button of button: MouseButton * state: MouseButtonState * position: Vector<pixel>

type GameEvent =
    | Settings of settings: SettingsEvent
    | ContentEvent of event: ContentEvent
    | Mouse of event: MouseEvent

type Game<'TState> (
                    init: unit -> 'TState, 
                    update: 'TState -> GameEvent list -> float32<second> -> ('TState * GameCommand list),
                    draw: 'TState -> float32<second> -> Graphics option) as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new GraphicsDeviceManager(this)

    let mutable state = init()
    let mutable mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState()
    let mutable events = []
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let delta (gameTime: GameTime) = (float32 gameTime.ElapsedGameTime.TotalSeconds * 1.0f<second>)

    let handleMouseInput (state: Microsoft.Xna.Framework.Input.MouseState) (state': Microsoft.Xna.Framework.Input.MouseState) =
        seq {
            yield if state'.Position   <> state.Position   then state'.Position |> Utils.pointToPixelVector |> MouseEvent.Moved |> Some else None
            yield if state'.LeftButton <> state.LeftButton then MouseEvent.Button(MouseButton.Left, state'.LeftButton |> MouseInput.toButtonState,  Utils.pointToPixelVector state'.Position) |> Some else None
        } |> Seq.filter Option.isSome |> Seq.map Option.get |> Seq.map GameEvent.Mouse |> Seq.toList

    let handleCommand command =
        match command with 
        | SettingsCommand settings -> 
            match settings with 
            | UpdateScreenSize(width, height) -> 
                graphics.PreferredBackBufferWidth  <- width
                graphics.PreferredBackBufferHeight <- height
                graphics.ApplyChanges();

                Some (GameEvent.Settings(ScreenSizeUpdated(width, height)))
            | _ -> None

        | ContentCommand command ->
            match command with 
            | LoadFont path -> 
                let font = this.Content.Load<SpriteFont>(path) |> Font

                Some <| GameEvent.ContentEvent(FontLoaded(path, font))
            | LoadTexture path -> 
                let texture = this.Content.Load<Texture2D>(path) |> Texture

                Some <| GameEvent.ContentEvent(TextureLoaded(path, texture))

        | ExitCommand -> 
            this.Exit()
            None

    let handleCommands commands = commands |> List.map handleCommand |> List.filter Option.isSome |> List.map Option.get

    override _.LoadContent() =
        this.Content.RootDirectory <- "Content"
       
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

    override this.Initialize () =

        base.Initialize()
        
        this.Window.AllowUserResizing <- true

        base.IsMouseVisible <- true

    override _.Update (gameTime: GameTime) =

        match this.IsActive with 
        | true -> 
            let mouseState' = Mouse.GetState()
            let mouseEvents = handleMouseInput mouseState mouseState'
            mouseState <- mouseState'

            let (state', commands) = update state (events@mouseEvents) (delta gameTime)

            state <- state'
            events <- commands |> handleCommands

        | false -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: GameTime) =
        
        match draw state (delta gameTime) with
        | Some g -> 
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue)
            Graphics.draw spriteBatch g
        | _ -> ()

module Game = 
    let run (init: unit -> 'TState) (update: 'TState -> GameEvent list -> float32<second> ->  ('TState * GameCommand list)) (draw: 'TState -> float32<second> -> Graphics option) = 
        let game = new Game<'TState>(init, update, draw)
        game.Run()