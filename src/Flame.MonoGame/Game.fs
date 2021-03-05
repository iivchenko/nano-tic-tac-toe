namespace Flame.MonoGame

open Flame
open Flame.Content
open Flame.Input
open Flame.MonoGame.Input
open System.Collections.Generic

type Game<'TState> (
                    init: unit -> 'TState, 
                    update: 'TState -> GameEvent list -> float32<second> -> ('TState * GameCommand list)) as this =

    inherit Microsoft.Xna.Framework.Game()

    let graphics = new XnaGraphicsDeviceManager(this)
    let content = new Dictionary<string, System.Object>()

    let mutable state = init()
    let mutable mouseState = XnaMouse.GetState()
    let mutable events = []
    let mutable commands = []
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
            let xnaSound = this.Content.Load<XnaSound>(path)
            
            // TODO: Remove this hack when Scenes will have unload or destroy method or something else
            if content.ContainsKey(path)
                then content.[path] <- xnaSound
                else content.Add(path, xnaSound)
            let sound = Sound(path)
            Some <| SoundLoadedEvent(path, sound)

        | LoadSongCommand path -> 
            let xnaSong = this.Content.Load<XnaSong>(path)
            content.Add(path, xnaSong)
            let song = Song(path)
            Some <| SongLoadedEvent(path, song)

        | PlaySoundCommand (Sound(id)) ->
            let xnaSound = content.[id] :?> XnaSound
            do xnaSound.Play() |> ignore
            None

        | PlaySongCommand (Song(id)) -> 
            let xnaSong = content.[id] :?> XnaSong
            do XnaMediaplayer.Play(xnaSong)
            None

        | ExitGameCommand -> 
            do this.Exit()
            None
        | DrawCommand(_) -> 
            failwith "This command shoudl not be handled here!" // TODO: Improve

    let handleCommands commands = commands |> List.map handleCommand |> List.filter Option.isSome |> List.map Option.get

    let isDraw command =
        match command with
        | DrawCommand(_) -> true
        | _ -> false

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

            let (state', commands') = update state (events@mouseEvents) (delta gameTime)

            commands <- commands'
            state <- state'
            events <- commands |> List.where (fun x -> x |> isDraw |> not)  |> handleCommands

        | false -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: XnaGameTime) =

        graphics.GraphicsDevice.Clear(XnaColor.White)
        commands |> List.where (fun x -> x |> isDraw)  |> List.iter (fun (DrawCommand(g)) -> Graphics.draw spriteBatch g content)

module Game = 
    let run (init: unit -> 'TState) (update: 'TState -> GameEvent list -> float32<second> ->  ('TState * GameCommand list)) = 
        let game = new Game<'TState>(init, update)
        game.Run()