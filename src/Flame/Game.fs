namespace Flame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Flame.Content

type GameSettings =
    { ScreenWidth: int
      ScreenHeight: int }

type GameApi = 
    { LoadFont: string -> Font 
      LoadTexture: string -> Texture }

type GameState<'TState> =
    { Api: GameApi
      State: 'TState }

type GameEvent<'TState> = 
    | None of state: 'TState
    | Exit

type Game<'TState> (
                    settings: GameSettings, 
                    state: 'TState, 
                    update: GameState<'TState> -> float32<second> -> GameEvent<'TState>,
                    draw: 'TState -> float32<second> -> unit) as this =
    inherit Microsoft.Xna.Framework.Game()

    let graphics = new GraphicsDeviceManager(this)

    let mutable state = 
                    { 
                        Api = 
                            {
                                LoadFont    = (fun name -> this.Content.Load<SpriteFont>(name) |> Font)
                                LoadTexture = (fun name -> this.Content.Load<Texture2D>(name)  |> Texture)
                            }
                        State = state
                    }
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let delta (gameTime: GameTime) = (float32 gameTime.ElapsedGameTime.TotalSeconds * 1.0f<second>)

    override _.LoadContent() =
        this.Content.RootDirectory <- "Content"
       
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

    override this.Initialize () =

        base.Initialize()

        graphics.PreferredBackBufferWidth  <- settings.ScreenWidth
        graphics.PreferredBackBufferHeight <- settings.ScreenHeight
        
        this.Window.AllowUserResizing <- true

        graphics.ApplyChanges();

        base.IsMouseVisible <- true

    override _.Update (gameTime: GameTime) =

        match this.IsActive with 
        | true -> 
            match update state (delta gameTime) with
            | None state' -> state <- { state with State = state' }
            | Exit -> this.Exit()
        | false -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: GameTime) =
        
        graphics.GraphicsDevice.Clear(Color.CornflowerBlue)
        
        draw state.State (delta gameTime)


module Game = 
    let run settings (state: 'TState) (update: GameState<'TState> -> float32<second> -> GameEvent<'TState>) (draw: 'TState -> float32<second> -> unit) = 
        let game = new Game<'TState>(settings, state, update, draw)
        game.Run()