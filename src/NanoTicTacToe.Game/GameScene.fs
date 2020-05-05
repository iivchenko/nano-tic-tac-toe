namespace NanoTicTacToe.Game

module GameScene = 

    let init () = InitMainMenu { Resources = [] }

    let update state events _ = 

        match state with 
        | MainMenuScene data -> MainMenuScene.update data events
        | GamePlayScene state -> GamePlayScene.update state events
        | InitGamePlay data -> GamePlayScene.init data events
        | InitMainMenu data -> MainMenuScene.init data events

    let draw state _ =

        match state with 
        | MainMenuScene data -> MainMenuScene.draw data |> Some
        | GamePlayScene state -> GamePlayScene.draw state |> Some
        | InitMainMenu _
        | InitGamePlay _ -> None