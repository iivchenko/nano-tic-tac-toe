open NanoTicTacToe.Game
open Flame.MonoGame

let private init () = MainMenuScene (MainMenuInitState { FirstRun = true })

[<EntryPoint>]
let main _ =

    Game.run init GameScene.update

    0