open Flame

open NanoTicTacToe.Game

[<EntryPoint>]
let main _ =
    let state = GameState.InitMainMenu { Resources = [] }

    Game.run state Scene.update Scene.draw

    0