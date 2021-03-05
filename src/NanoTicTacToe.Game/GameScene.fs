namespace NanoTicTacToe.Game

module GameScene = 

    let init () = InitMainMenu { FirstRun = true }

    let update state events delta = 

        match state with 
        | MainMenuScene data -> MainMenuScene.update data events
        | GamePlayScene state -> GamePlayScene.update state events delta
        | InitGamePlay data -> GamePlayScene.init data events
        | InitMainMenu data -> MainMenuScene.init data events