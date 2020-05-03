namespace NanoTicTacToe.Game

open Flame

type Scene = 
    | EntryScene 
    | MainMenuScene of state: MainMenuSceneState
    | GamePlayScene of state: GamePlaySceneState

type SceneState = 
    { Scene: Scene }

module Scene = 

    let update state events _ = 
        let api = state.Api
        let gameState = state.State
        let settings = state.Settings
        let sceneState = state.State.Scene

        match sceneState with 
        | MainMenuScene scene -> 
            match MainMenuScene.update scene events with 
            | MainMenuEvent.None state -> { gameState with Scene = (MainMenuScene state) } |> GameCommand.None
            | MainMenuEvent.StartGame -> { gameState with Scene = (GamePlayScene (GamePlayScene.init api settings)) } |> GameCommand.None
            | MainMenuEvent.Exit -> GameCommand.Exit
        | GamePlayScene state ->  { gameState with Scene = (GamePlayScene(GamePlayScene.update state events)) } |> GameCommand.None
        | EntryScene ->
            { gameState with Scene = (MainMenuScene(MainMenuScene.init api settings)) } |> GameCommand.None

    let draw state _ =
        match state.Scene with 
        | MainMenuScene state -> MainMenuScene.draw state
        | GamePlayScene state -> GamePlayScene.draw state
        | _ -> raise (new exn("Unknown Scene!"))