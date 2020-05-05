open Flame

open NanoTicTacToe.Game

[<EntryPoint>]
let main _ =

    Game.run GameScene.init GameScene.update GameScene.draw

    0