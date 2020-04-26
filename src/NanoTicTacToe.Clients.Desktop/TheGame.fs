namespace NanoTicTacToe.Clients.Desktop

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Myra

type TheGame () as this =
    inherit Game()

    let width = 1920
    let height = 1080
    let graphics = new GraphicsDeviceManager(this)

    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    override _.LoadContent() =
        this.Content.RootDirectory <- "Content"
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)

    override this.Initialize () =

        base.Initialize()

        graphics.PreferredBackBufferWidth  <- width
        graphics.PreferredBackBufferHeight <- height
        
        this.Window.AllowUserResizing <- true

        graphics.ApplyChanges();

        base.IsMouseVisible <- true

        MyraEnvironment.Game <- this   

    override _.Update (gameTime: GameTime) =

        match this.IsActive with 
        | true -> ()
        | _ -> ()

        base.Update(gameTime)

    override _.Draw (gameTime: GameTime) =
        
        graphics.GraphicsDevice.Clear(Color.CornflowerBlue)
        
        ()