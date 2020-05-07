namespace NanoTicTacToe.Game

open Flame
open Flame.Content
open Flame.Graphics
open Flame.Input

module MainMenuScene = 

    let private mouseClick events = events |> List.tryFind (fun event -> match event with | MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, _) -> true | _ -> false)

    let getFont path events = 
        let (_, font) = events |> List.map (fun event -> match event with | FontLoadedEvent(p, font) when path = p -> Some (p, font) | _ -> None) |> List.filter Option.isSome |> List.map Option.get |> List.exactlyOne
        font

    let getSound path events = 
        let (_, sound) = events |> List.map (fun event -> match event with | SoundLoadedEvent(p, sound) when path = p -> Some (p, sound) | _ -> None) |> List.filter Option.isSome |> List.map Option.get |> List.exactlyOne
        sound

    let init (state: MainMenuInitState) events = 

        if state.FirstRun 
            then 
                (InitMainMenu({ state with FirstRun = false; }), [LoadFontCommand "Fonts/H1"; LoadFontCommand "Fonts/H2"; LoadSoundCommand "SoundFX/button-click"])
            else 
                let screenWidth = 1920.0f<pixel>
                let screenHeight = 1080.0f<pixel>
                let h1 = getFont "Fonts/H1" events
                let h2 = getFont "Fonts/H2" events

                let header = "Tic Tac Toe"
                let (Vector(width, height)) = Font.length h1 header
                let headerP = Vector.init (screenWidth / 2.0f - width / 2.0f) (screenHeight * 0.40f)

                let start = "Start"
                let (Vector(startW, startH)) = Font.length h2 start
                let startP = Vector.init (screenWidth / 2.0f - startW / 2.0f) (screenHeight * 0.50f)

                let exit = "Exit"
                let (Vector(exitW, exitH)) = Font.length h2 exit
                let exitP = Vector.init (screenWidth / 2.0f - exitW / 2.0f) (screenHeight * 0.51f + exitH)

                let sound = getSound "SoundFX/button-click" events

                (MainMenuScene({ Header = Text(headerP, h1, Color.white, header); Start = Text(startP, h2, Color.white, start); Exit = Text(exitP, h2, Color.white, exit); ClickSound = sound }), [])

    let update state events =

        match mouseClick events with 
        | Some(MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, position)) when Graphics.inBounds position state.Exit  -> (GameState.MainMenuScene state, [PlaySoundCommand state.ClickSound; ExitGameCommand])
        | Some(MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, position)) when Graphics.inBounds position state.Start -> (GameState.InitGamePlay({ FirstRun = true; }), [PlaySoundCommand state.ClickSound])
        | _ -> (GameState.MainMenuScene state, [])

    let draw state = Graphics([state.Header; state.Start; state.Exit])