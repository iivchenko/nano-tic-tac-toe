namespace NanoTicTacToe.Game

open Flame.Content
open Flame.Graphics

type MainMenuInitState = 
    { FirstRun: bool }

type MainMenuSceneState = 
    { Header: Graphics
      Start:  Graphics
      Exit:   Graphics 
      ClickSound: Sound }