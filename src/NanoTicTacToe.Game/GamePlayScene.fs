namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content
open Flame.Input
open Flame.MonoGame

module Grid = 
    let raw grid index = grid |> List.where (fun (raw, _, _) -> raw = index)
    let column grid index = grid |> List.where (fun (_, column, _) -> column = index)
    let update grid raw column cell = grid |> List.map (fun (x, y, i) -> if raw = x && column = y then (x, y, cell) else (x, y, i))
    let get grid raw column = grid |> List.filter (fun (x, y, _) -> raw = x && column = y) |> List.map (fun (_, _, cell) -> cell) |> List.exactlyOne

module AI = 
    let move grid =
        let (raw, column, _) = grid |> List.filter (fun (_, _, cell) -> match cell with | Empty _ -> true | _ -> false) |> List.maxBy (fun (_, _, (Empty value)) -> value)
        (raw, column)

module GamePlayScene = 

    let private initGrid = 
        [ 
            (0, 0, Empty 3); (0, 1, Empty 2); (0, 2, Empty 3);
            (1, 0, Empty 2); (1, 1, Empty 4); (1, 2, Empty 2);  
            (2, 0, Empty 3); (2, 1, Empty 2); (2, 2, Empty 3);
        ]

    let private lines =
        [
            // Raws
            [(0, 0); (0, 1); (0, 2)];
            [(1, 0); (1, 1); (1, 2)];
            [(2, 0); (2, 1); (2, 2)];

            // Columns
            [(0, 0); (1, 0); (2, 0)];
            [(0, 1); (1, 1); (2, 1)];
            [(0, 2); (1, 2); (2, 2)];

            // Main diagonal
            [(0, 0); (1, 1); (2, 2)];

            // Second diagonal
            [(2, 0); (1, 1); (0, 2)];
        ]

    let onMouseButtonEvent check action defaultAction events = 
    
        let event = events |> List.tryFind (fun event -> match event with | MouseButtonEvent(_, _, _) -> true | _ -> false)

        match event with
        | Some(MouseButtonEvent(button, state, position)) when check button state position -> action button state position
        | _ -> defaultAction()

    let private map raw column value font xs os (Vector(x, y)) (width:float32<pixel>) (height:float32<pixel>) =
        match value with
        | Empty v -> 
            let (Vector(fW, fH)) = Font.length font (sprintf "%i" v)
            let x = x + width * column + width / 2.0f - fW / 2.0f
            let y = y + height * raw + height / 2.0f - fH / 2.0f
            Text(Vector.init x y, font, Color.grey, sprintf "%i" v)
        | Occupied(_, sym) ->
            match sym with 
            | X -> 
                let (Vector(xW, xH)) = Texture.size xs
                let x = x + width  * column + width / 2.0f - xW / 2.0f
                let y = y + height * raw + height / 2.0f - xH / 2.0f
                Sprite(Vector.init x y, xs, Vector.init 1.0f 1.0f)
            | O -> 
                let (Vector(oW, oH)) = Texture.size os
                let x = x + width  * column + width / 2.0f - oW / 2.0f
                let y = y + height * raw + height / 2.0f - oH / 2.0f
                Sprite(Vector.init x y, os, Vector.init 1.0f 1.0f)

    let private (| PlayerWon | AiWon | Tie | ContinueGame |) grid =
        let winLine player grid line = 
            line 
            |> List.map (fun (raw, column) -> Grid.get grid raw column) 
            |> List.forall (fun cell -> match cell with | Occupied(p, _) -> p = player | _ -> false)

        let win (grid: (int*int*Cell) list) (player: Player) = lines |> List.exists (winLine player grid)

        if win grid Player then PlayerWon
        elif win grid AI then AiWon
        elif grid |> List.exists (fun (_, _, cell) -> match cell with | Empty _ -> true | _ -> false) then ContinueGame
        else Tie

    let private makeMove state position = 
        let (Vector(x,y)) = position
        let (Vector(xo, yo)) = state.Origin
        let column = (x - xo) / state.Back.CellWidth |> int
        let raw = (y - yo) / state.Back.CellHeight |> int
        let cell = Grid.get state.Grid raw column 

        match cell with
        | Empty _ -> PlayerMoved <| Grid.update state.Grid raw column (Occupied(state.CurrentPlayer, X))
        | _       -> PlayerThinking

    let private nextPlayer = function | Player -> AI | AI -> Player

    let private message state text = 
        let (Vector(width, _)) = Font.length state.Content.Font text
        let x = 1920.0f<pixel> / 2.0f - width / 2.0f
        let y = 1080.0f<pixel> * 0.1f

        Text(Vector.init x y, state.Content.Font, Color.red, text)

    let private play   = Play   >> GamePlayScene
    let private finish = Finish >> GamePlayScene
    let private restart state = play { state with Grid = initGrid; }

    let init state events = 
        let getFont path events = 
            let (_, font) = events |> List.map (fun event -> match event with | FontLoadedEvent(p, font) when path = p -> Some (p, font) | _ -> None) |> List.filter Option.isSome |> List.map Option.get |> List.exactlyOne
            font

        let getSound path events = 
            let (_, sound) = events |> List.map (fun event -> match event with | SoundLoadedEvent(p, sound) when path = p -> Some (p, sound) | _ -> None) |> List.filter Option.isSome |> List.map Option.get |> List.exactlyOne
            sound

        let getTexture path events = 
            let (_, texture) = events |> List.map (fun event -> match event with | TextureLoadedEvent(p, texture) when path = p -> Some (p, texture) | _ -> None) |> List.filter Option.isSome |> List.map Option.get |> List.exactlyOne
            texture

        if state.FirstRun 
            then 
                (InitGamePlay({ FirstRun = false; }), [LoadFontCommand "Fonts/H1"; LoadTextureCommand "Sprites/Back"; LoadTextureCommand "Sprites/X"; LoadTextureCommand "Sprites/O"; LoadSoundCommand "SoundFX/button-click"])
            else 
                let screenWidth = 1920.0f<pixel>
                let screenHeight = 1080.0f<pixel>

                let back = getTexture "Sprites/Back" events
                let x = getTexture "Sprites/X" events
                let o = getTexture "Sprites/O" events
                let font = getFont "Fonts/H1" events
                let sound = getSound "SoundFX/button-click" events

                let (Vector(width, height)) =  Texture.size back
                let position = Vector.init ((screenWidth / 2.0f) - (width / 2.0f)) ((screenHeight / 2.0f) - (height / 2.0f))

                let state =
                    { 
                        Content = { X = x; O = o; Font = font; MoveSound = sound; }
                        Grid = initGrid
                        Origin = position
                        CurrentPlayer = Player
                        Back = { Sprite = Sprite(position, back, Vector.init 1.0f 1.0f); CellWidth = width / 3.0f; CellHeight = height / 3.0f }
                        FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, font, Color.red, ""))
                        MoveDelay = 0.0f<second>
                    }
                (play state,[])

    let update state events delta =
        match state with
        | Play state when state.MoveDelay > 0.0f<second> ->
            (play { state with MoveDelay = state.MoveDelay - delta }, [])
        | Play state -> 
            let status = 
                match state.CurrentPlayer with
                | Player -> 
                    let back = state.Back.Sprite
                    onMouseButtonEvent 
                        (fun button state position -> button = MouseButton.Left && state = MouseButtonState.Released && Graphics.inBounds position back) 
                        (fun _ _ position -> makeMove state position) 
                        (fun () -> PlayerThinking) 
                        events

                | AI -> 
                    let (raw, column) = AI.move state.Grid 
                    Grid.update state.Grid raw column (Occupied(AI, O)) |> PlayerMoved
            
            match status with 
            | PlayerThinking -> (play state, [])
            | PlayerMoved grid -> 
                let player = nextPlayer state.CurrentPlayer
                match grid with 
                | ContinueGame -> (play   { state with Grid = grid; CurrentPlayer = player; MoveDelay = 0.35f<second> }, [PlaySoundCommand state.Content.MoveSound])
                | Tie          -> (finish { state with Grid = grid; CurrentPlayer = player; FinishMessage = message state "A draw" }, [])
                | PlayerWon    -> (finish { state with Grid = grid; CurrentPlayer = player; FinishMessage = message state "Victory" }, [])
                | AiWon        -> (finish { state with Grid = grid; CurrentPlayer = player; FinishMessage = message state "Defeat"; }, [])

        | Finish state -> 
            onMouseButtonEvent
                (fun _ state _ -> state = MouseButtonState.Released) 
                (fun _ _ _     -> (restart state, [])) 
                (fun ()        -> (finish state, [])) 
                events

    let draw state =
        match state with
        | Play state -> 
            let grid = state.Grid
                    |> List.map (fun (raw, column, cell) -> map (float32 raw) (float32 column) cell state.Content.Font state.Content.X state.Content.O state.Origin state.Back.CellWidth state.Back.CellHeight)
                    |> Seq.cast<Graphics>
                    |> Seq.toList
                    |> Graphics
        
            Graphics(state.Back.Sprite::grid::[])
        | Finish state -> 
            let grid = state.Grid
                    |> List.map (fun (raw, column, cell) -> map (float32 raw) (float32 column) cell state.Content.Font state.Content.X state.Content.O state.Origin state.Back.CellWidth state.Back.CellHeight)
                    |> Seq.cast<Graphics>
                    |> Seq.toList
                    |> Graphics

            Graphics(state.Back.Sprite::grid::state.FinishMessage::[])