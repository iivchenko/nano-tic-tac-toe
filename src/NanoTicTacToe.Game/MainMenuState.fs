namespace NanoTicTacToe.Game

open Flame.Content
open Flame.Graphics

type MainMenuSceneInitState = 
    { FirstRun: bool }

type MainMenuUpdateState = 
    { Header: Graphics
      Start:  Graphics
      Exit:   Graphics 
      ClickSound: Sound }

type MainMenuSceneState =
    | MainMenuInitState of state: MainMenuSceneInitState 
    | MainMenuUpdateState of state: MainMenuUpdateState