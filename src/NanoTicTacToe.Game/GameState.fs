namespace NanoTicTacToe.Game

type GameState =
    | MainMenuScene of state: MainMenuSceneState
    | InitGamePlay  of state: GamePlayInitState
    | GamePlayScene of state: GamePlaySceneState