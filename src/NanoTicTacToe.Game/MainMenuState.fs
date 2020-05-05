namespace NanoTicTacToe.Game

open Flame.Content
open Flame.Graphics

type MainMenuInitState = 
    { Resources: (string * Font) list }

type MainMenuSceneState = 
    { Header: Graphics
      Start:  Graphics
      Exit:   Graphics }