namespace Flame.MonoGame

open Flame
open Flame.Content
open Flame.Graphics
open Flame.Input
open Flame.MonoGame.Input
open System.Collections.Generic

type GameCommand =
    | UpdateScreenSizeCommand of width: int * height: int
    | LoadFontCommand         of path: string
    | LoadTextureCommand      of path: string
    | LoadSoundCommand        of path: string
    | LoadSongCommand         of path: string
    | PlaySoundCommand        of sound: Sound
    | PlaySongCommand         of song: Song
    | ExitGameCommand

type GameEvent =
    | ScreenSizeUpdatedEvent of width: int * height: int
    | FontLoadedEvent        of path: string * sprite: Font
    | TextureLoadedEvent     of path: string * texture: Texture
    | SoundLoadedEvent       of path: string * sound: Sound
    | SongLoadedEvent        of path: string * song: Song
    | MouseMovedEvent        of position: Vector<pixel>
    | MouseButtonEvent       of button: MouseButton * state: MouseButtonState * position: Vector<pixel>

type Game<'TState> (
                    init: unit -> 'TState, 
                    update: 'TState -> GameEvent list -> float32<second> -> ('TState * GameCommand list),
                    draw: 'TState -> float32<second> -> Graphics option) as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new XnaGraphicsDeviceManager(this)
    let content = new Dictionary<string, XnaTexture>()

    let mutable state = init()
    let mutable mouseState = XnaMouse.GetState()
    let mutable events = []
    let mutable spriteBatch = Unchecked.defaultof<XnaSpriteBatch>

    let delta (gameTime: XnaGameTime) = (float32 gameTime.ElapsedGameTime.TotalSeconds * 1.0f<second>)

    let handleMouseInput (state: XnaMouseState) (state': XnaMouseState) =
        seq {
            yield if state'.Position   <> state.Position   then state'.Position |> Utils.pointToPixelVector |> MouseMovedEvent |> Some else None
            yield if state'.LeftButton <> state.LeftButton then MouseButtonEvent(MouseButton.Left, state'.LeftButton |> MonoGameMouseInput.toButtonState, Utils.pointToPixelVector state'.Position) |> Some else None
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
            let xnaTexture = this.Content.Load<XnaTexture>(path) 
            content.Add(path, xnaTexture)
            let texture = Texture(path, xnaTexture.Width |> float32 |> (*) 1.0f<pixel>, xnaTexture.Height |> float32 |> (*) 1.0f<pixel>)
            Some <| TextureLoadedEvent(path, texture)

        | LoadSoundCommand path -> 
            let sound = this.Content.Load<XnaSound>(path) |> Sound
            Some <| SoundLoadedEvent(path, sound)

        | LoadSongCommand path -> 
            let song = this.Content.Load<XnaSong>(path) |> Song
            Some <| SongLoadedEvent(path, song)

        | PlaySoundCommand (Sound(sound)) ->
            do sound.Play() |> ignore
            None

        | PlaySongCommand (Song(song)) -> 
            do XnaMediaplayer.Play(song)
            None

        | ExitGameCommand -> 
            do this.Exit()
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
            graphics.GraphicsDevice.Clear(XnaColor.White)
            Graphics.draw spriteBatch g content
        | _ -> ()

module Game = 
    let run (init: unit -> 'TState) (update: 'TState -> GameEvent list -> float32<second> ->  ('TState * GameCommand list)) (draw: 'TState -> float32<second> -> Graphics option) = 
        let game = new Game<'TState>(init, update, draw)
        game.Run()