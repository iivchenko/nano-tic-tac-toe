﻿namespace NanoTicTacToe.Game

open Flame
open Flame.Content
open Flame.Graphics
open Flame.Input

type MainMenuSceneState = 
    { Header: Graphics 
      Start:  Graphics 
      Exit:   Graphics 
      PreivosMouseLeftButton: MouseButtonState }

type MainMenuEvent =
    | None of state: MainMenuSceneState
    | StartGame
    | Exit

module MainMenuScene = 

    let private mouseClick events = events |> List.tryFind (fun event -> match event with | GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, _)) -> true | _ -> false)

    let init api settings = 
        let h1 = api.LoadFont "Fonts/H1"
        let h2 = api.LoadFont "Fonts/H2"

        let header = "Tic Tac Toe"
        let (Vector(width, height)) = Font.length h1 header
        let headerP = Vector.init (settings.ScreenWidth / 2.0f - width / 2.0f) (settings.ScreenHeight * 0.40f)

        let start = "Start"
        let (Vector(startW, startH)) = Font.length h2 start
        let startP = Vector.init (settings.ScreenWidth / 2.0f - startW / 2.0f) (settings.ScreenHeight * 0.50f)

        let exit = "Exit"
        let (Vector(exitW, exitH)) = Font.length h2 exit
        let exitP = Vector.init (settings.ScreenWidth / 2.0f - exitW / 2.0f) (settings.ScreenHeight * 0.51f + exitH)

        { Header = Text(headerP, h1, Color.white, header); Start = Text(startP, h2, Color.white, start); Exit = Text(exitP, h2, Color.white, exit); PreivosMouseLeftButton = MouseButtonState.Released; }

    let update state events =
        match mouseClick events with 
        | Some(GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, position))) when Graphics.inBounds position state.Exit  -> MainMenuEvent.Exit
        | Some(GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, position))) when Graphics.inBounds position state.Start -> MainMenuEvent.StartGame
        | _ -> MainMenuEvent.None state

    let draw state = Graphics([state.Header; state.Start; state.Exit])