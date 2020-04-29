namespace NanoTicTacToe.Game

open Flame

type Scene = 
    | EntryScene 
    | MainMenuScene of state: MainMenuSceneState
    | GamePlayScene of state: GamePlaySceneState

type SceneState = 
    { Scene: Scene }

module Scene = 

    let update state inputs delta = 
        let api = state.Api
        let gameState = state.State
        let settings = state.Settings
        let sceneState = state.State.Scene

        match sceneState with 
        | MainMenuScene scene -> 
            match MainMenuScene.update scene inputs with 
            | MainMenuEvent.None state -> { gameState with Scene = (MainMenuScene state) } |> GameEvent.None
            | MainMenuEvent.StartGame -> GameEvent.Exit
            | MainMenuEvent.Exit -> GameEvent.Exit
        | GamePlayScene state ->  { gameState with Scene = (GamePlayScene(GamePlayScene.update state delta)) } |> GameEvent.None
        | EntryScene ->
            { gameState with Scene = (MainMenuScene(MainMenuScene.init api settings)) } |> GameEvent.None

    let draw state delta =
        match state.Scene with 
        | MainMenuScene state -> MainMenuScene.draw state
        | GamePlayScene state -> GamePlayScene.draw state delta
        | _ -> raise (new exn("Unknown Scene!"))