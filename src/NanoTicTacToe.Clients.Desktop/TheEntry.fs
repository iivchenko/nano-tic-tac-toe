open NanoTicTacToe.Game
open Flame.MonoGame

[<EntryPoint>]
let main _ =

    Game.run GameScene.init GameScene.update

    0