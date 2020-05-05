namespace NanoTicTacToe.Game

type GameState = 
    | InitMainMenu  of state: MainMenuInitState
    | MainMenuScene of state: MainMenuSceneState
    | InitGamePlay  of state: GamePlayInitState
    | GamePlayScene of state: GamePlaySceneState