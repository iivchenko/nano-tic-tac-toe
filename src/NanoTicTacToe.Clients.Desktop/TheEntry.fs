open Flame

open NanoTicTacToe.Game

[<EntryPoint>]
let main _ =
    let state = GameState.InitMainMenu { Resources = [] }

    Game.run state GameScene.update GameScene.draw

    0