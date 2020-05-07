namespace NanoTicTacToe.Game

open Flame
open Flame.Content
open Flame.Graphics
open Flame.Input

module MainMenuScene = 

    let private mouseClick events = events |> List.tryFind (fun event -> match event with | MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, _) -> true | _ -> false)

    let init state events = 

        let fonts = events |> List.map (fun event -> match event with | FontLoadedEvent(path, font) -> Some (path, font) | _ -> None) |> List.filter Option.isSome |> List.map Option.get
        let resources = state.Resources@fonts

        if resources |> List.exists (fun (path, _) -> path = "Fonts/H1") |> not
            then (InitMainMenu({ state with Resources = resources; }), [LoadFontCommand "Fonts/H1"])
        elif resources |> List.exists (fun (path, _) -> path = "Fonts/H2") |> not
            then (InitMainMenu({ state with Resources = resources; }), [LoadFontCommand "Fonts/H2"])
        else 
            let screenWidth = 1920.0f<pixel>
            let screenHeight = 1080.0f<pixel>
            let h1 = resources |> List.filter (fun (path, _) -> path = "Fonts/H1") |> List.map (fun (_, font) -> font) |> List.exactlyOne
            let h2 = resources |> List.filter (fun (path, _) -> path = "Fonts/H2") |> List.map (fun (_, font) -> font) |> List.exactlyOne

            let header = "Tic Tac Toe"
            let (Vector(width, height)) = Font.length h1 header
            let headerP = Vector.init (screenWidth / 2.0f - width / 2.0f) (screenHeight * 0.40f)

            let start = "Start"
            let (Vector(startW, startH)) = Font.length h2 start
            let startP = Vector.init (screenWidth / 2.0f - startW / 2.0f) (screenHeight * 0.50f)

            let exit = "Exit"
            let (Vector(exitW, exitH)) = Font.length h2 exit
            let exitP = Vector.init (screenWidth / 2.0f - exitW / 2.0f) (screenHeight * 0.51f + exitH)
            
            (MainMenuScene({ Header = Text(headerP, h1, Color.white, header); Start = Text(startP, h2, Color.white, start); Exit = Text(exitP, h2, Color.white, exit); }), [])

    let update state events =
        match mouseClick events with 
        | Some(MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, position)) when Graphics.inBounds position state.Exit  -> (GameState.MainMenuScene state, [ExitGameCommand])
        | Some(MouseButtonEvent(MouseButton.Left, MouseButtonState.Released, position)) when Graphics.inBounds position state.Start -> (GameState.InitGamePlay({ Textures = []; Fonts = [] }), [])
        | _ -> (GameState.MainMenuScene state, [])

    let draw state = Graphics([state.Header; state.Start; state.Exit])