namespace NanoTicTacToe.Game

module GameScene = 

    let update state events delta = 

        match state with 
        | MainMenuScene data -> MainMenuScene.update data events
        | GamePlayScene state -> GamePlayScene.update state events delta
        | InitGamePlay data -> GamePlayScene.init data events