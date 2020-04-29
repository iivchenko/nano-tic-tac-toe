namespace NanoTicTacToe.Game

open Flame.Graphics

type GamePlaySceneContent =
    { Dummy: int }

type GamePlaySceneState = 
    { Content:  GamePlaySceneContent 
      Dummy: Graphics }

type GamePlayEvent =
    | None of state:GamePlaySceneState
    | Exit

module GamePlayScene = 
    let init api = ()
    let update state delta = state
    let draw state delta = state.Dummy