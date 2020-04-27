﻿namespace NanoTicTacToe.Game

type GamePlaySceneContent =
    { Dummy: int }

type GamePlaySceneState = 
    { Content:  GamePlaySceneContent }

type GamePlayEvent =
    | None of state:GamePlaySceneState
    | Exit

module GamePlayScene = 
    let init api = ()
    let update state delta = state
    let draw state delta = ()