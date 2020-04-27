namespace NanoTicTacToe.Game

open Flame
open Flame.Content

type MeinMenuSceneContent =
    { H1: Font 
      H3: Font }

type MainMenuSceneState = 
    { Content:  MeinMenuSceneContent }

type MainMenuEvent =
    | None of state: MainMenuSceneState
    | StartGame
    | Exit

module MainMenuScene = 
    let init api = { Content = { H1 = api.LoadFont "H1"; H3 = api.LoadFont "H3" } }
    let update state delta = MainMenuEvent.None state
    let draw state delta = ()