open NanoTicTacToe.Game
open Flame.MonoGame

[<EntryPoint>]
let main _ =

    Game.run GameScene.init GameScene.update GameScene.draw

    0