namespace NanoTicTacToe.Game

open Flame

type Scene = 
    | EntryScene 
    | MainMenuScene of state: MainMenuSceneState
    | GamePlayScene of state: GamePlaySceneState

type SceneState = 
    { Scene: Scene }

module Scene = 

    let update state delta = 
        let api = state.Api
        let gameState = state.State
        let sceneState = state.State.Scene

        match sceneState with 
        | MainMenuScene scene -> 
            match MainMenuScene.update scene delta with 
            | MainMenuEvent.None state -> { gameState with Scene = (MainMenuScene state) } |> GameEvent.None
            | MainMenuEvent.StartGame -> GameEvent.Exit
            | MainMenuEvent.Exit -> GameEvent.Exit
        | GamePlayScene state ->  { gameState with Scene = (GamePlayScene(GamePlayScene.update state delta)) } |> GameEvent.None
        | EntryScene ->
            { gameState with Scene = (MainMenuScene(MainMenuScene.init api)) } |> GameEvent.None

    let draw state delta =
        match state.Scene with 
        | MainMenuScene state -> MainMenuScene.draw state delta
        | GamePlayScene state -> GamePlayScene.draw state delta
        | _ -> ()