namespace NanoTicTacToe.Game

open Flame

module Scene = 

    let update state events api settings _ = 

        match state with 
        | MainMenuScene scene -> MainMenuScene.update scene events |> GameCommand.Continue
        | GamePlayScene state -> GamePlayScene.update state events |> GamePlayScene |> GameCommand.Continue
        | InitGamePlay -> GamePlayScene.init api settings |> GamePlayScene |> GameCommand.Continue
        | InitMainMenu -> MainMenuScene.init api settings |> MainMenuScene |> GameCommand.Continue
        | Scenes.Exit -> GameCommand.Exit

    let draw state _ =
        match state with 
        | MainMenuScene state -> MainMenuScene.draw state |> Some
        | GamePlayScene state -> GamePlayScene.draw state |> Some
        | InitMainMenu
        | InitGamePlay
        | Scenes.Exit -> None
        | _ -> raise (new exn("Unknown Scene!"))