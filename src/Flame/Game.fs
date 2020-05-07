namespace Flame

open Flame.Content
open Flame.Graphics
open Flame.Input

type XnaGraphicsDeviceManager = Microsoft.Xna.Framework.GraphicsDeviceManager
type XnaMouse = Microsoft.Xna.Framework.Input.Mouse
type XnaMouseState = Microsoft.Xna.Framework.Input.MouseState
type XnaSpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch
type XnaGameTime = Microsoft.Xna.Framework.GameTime
type XnaFont =  Microsoft.Xna.Framework.Graphics.SpriteFont
type XnaTexture = Microsoft.Xna.Framework.Graphics.Texture2D
type XnaColor =  Microsoft.Xna.Framework.Color

type GameCommand =
    | UpdateScreenSizeCommand of width: int * height: int
    | LoadFontCommand         of path: string
    | LoadTextureCommand      of path: string
    | ExitGameCommand

type GameEvent =
    | ScreenSizeUpdatedEvent of width: int * height: int
    | FontLoadedEvent        of path: string * sprite: Font
    | TextureLoadedEvent     of path: string * texture: Texture
    | MouseMovedEvent        of position: Vector<pixel>
    | MouseButtonEvent       of button: MouseButton * state: MouseButtonState * position: Vector<pixel>

type Game<'TState> (
                    init: unit -> 'TState, 
                    update: 'TState -> GameEvent list -> float32<second> -> ('TState * GameCommand list),
                    draw: 'TState -> float32<second> -> Graphics option) as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new XnaGraphicsDeviceManager(this)

    let mutable state = init()
    let mutable mouseState = XnaMouse.GetState()
    let mutable events = []
    let mutable spriteBatch = Unchecked.defaultof<XnaSpriteBatch>

    let delta (gameTime: XnaGameTime) = (float32 gameTime.ElapsedGameTime.TotalSeconds * 1.0f<second>)

    let handleMouseInput (state: XnaMouseState) (state': XnaMouseState) =
        seq {
            yield if state'.Position   <> state.Position   then state'.Position |> Utils.pointToPixelVector |> MouseMovedEvent |> Some else None
            yield if state'.LeftButton <> state.LeftButton then MouseButtonEvent(MouseButton.Left, state'.LeftButton |> MouseInput.toButtonState, Utils.pointToPixelVector state'.Position) |> Some else None
        } |> Seq.filter Option.isSome |> Seq.map Option.get |> Seq.toList

    let handleCommand command =
        match command with 
        | UpdateScreenSizeCommand(width, height) -> 
            graphics.PreferredBackBufferWidth  <- width
            graphics.PreferredBackBufferHeight <- height
            graphics.ApplyChanges();
            Some <| ScreenSizeUpdatedEvent(width, height)

        | LoadFontCommand path -> 
            let font = this.Content.Load<XnaFont>(path) |> Font
            Some <| FontLoadedEvent(path, font)

        | LoadTextureCommand path -> 
            let texture = this.Content.Load<XnaTexture>(path) |> Texture
            Some <| TextureLoadedEvent(path, texture)

        | ExitGameCommand -> 
            this.Exit()
            None

    let handleCommands commands = commands |> List.map handleCommand |> List.filter Option.isSome |> List.map Option.get

    override _.LoadContent() =
        this.Content.RootDirectory <- "Content"       
        spriteBatch <- new XnaSpriteBatch(this.GraphicsDevice)

    override this.Initialize () =

        base.Initialize()
        
        this.Window.AllowUserResizing <- true // TODO: Implememt as command

        base.IsMouseVisible <- true // TODO: implement as command

    override _.Update (gameTime: XnaGameTime) =

        match this.IsActive with // TODO: Implement as command
        | true -> 
            let mouseState' = XnaMouse.GetState()
            let mouseEvents = handleMouseInput mouseState mouseState'
            mouseState <- mouseState'

            let (state', commands) = update state (events@mouseEvents) (delta gameTime)

            state <- state'
            events <- commands |> handleCommands

        | false -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: XnaGameTime) =
        
        match draw state (delta gameTime) with
        | Some g -> 
            graphics.GraphicsDevice.Clear(XnaColor.CornflowerBlue)
            Graphics.draw spriteBatch g
        | _ -> ()

module Game = 
    let run (init: unit -> 'TState) (update: 'TState -> GameEvent list -> float32<second> ->  ('TState * GameCommand list)) (draw: 'TState -> float32<second> -> Graphics option) = 
        let game = new Game<'TState>(init, update, draw)
        game.Run()