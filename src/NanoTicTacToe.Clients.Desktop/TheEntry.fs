open Flame

open NanoTicTacToe.Game

[<EntryPoint>]
let main _ =
    let settings = { ScreenWidth = 1920.0f<pixel>; ScreenHeight = 1080.0f<pixel> }
    let state =    { Scene = EntryScene }

    Game.run settings state Scene.update Scene.draw

    0