namespace NanoTicTacToe.Game

module Scene = 

    let update state events _ = 

        match state with 
        | MainMenuScene data -> MainMenuScene.update data events
        | GamePlayScene _ 
        | GamePlaySceneFinish _ -> GamePlayScene.update state events
        | InitGamePlay data -> GamePlayScene.init data events
        | InitMainMenu data -> MainMenuScene.init data events

    let draw state _ =
        match state with 
        | MainMenuScene data -> MainMenuScene.draw data |> Some
        | GamePlayScene _ | GamePlaySceneFinish _ -> GamePlayScene.draw state |> Some
        | InitMainMenu _
        | InitGamePlay _ -> None
        | _ -> raise (new exn("Unknown Scene!"))