namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content
open Flame.Input

module Grid = 
    let raw grid index = grid |> List.where (fun (raw, _, _) -> raw = index)
    let column grid index = grid |> List.where (fun (_, column, _) -> column = index)
    let update grid raw column cell = grid |> List.map (fun (x, y, i) -> if raw = x && column = y then (x, y, cell) else (x, y, i))
    let get grid raw column = grid |> List.filter (fun (x, y, _) -> raw = x && column = y) |> List.map (fun (_, _, cell) -> cell) |> List.exactlyOne

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

    let private map row column value font xs os (Vector(x, y)) (width:float32<pixel>) (height:float32<pixel>) =
        match value with
        | Empty v -> 
            let (Vector(fW, fH)) = Font.length font (sprintf "%i" v)
            let x = x + width * column + width / 2.0f - fW / 2.0f
            let y = y + height * row + height / 2.0f - fH / 2.0f
            Text(Vector.init x y, font, Color.grey, sprintf "%i" v)
        | Occupied(AI, a) 
        | Occupied(Player, a) ->
            match a with 
            | X -> 
                let (Vector(xW, xH)) = Texture.size xs
                let x = x + width  * column + width / 2.0f - xW / 2.0f
                let y = y + height * row + height / 2.0f - xH / 2.0f
                Sprite(Vector.init x y, xs, Vector.init 1.0f 1.0f)
            | O -> 
                let (Vector(oW, oH)) = Texture.size os
                let x = x + width  * column + width / 2.0f - oW / 2.0f
                let y = y + height * row + height / 2.0f - oH / 2.0f
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

    let private play   = Play   >> GamePlayScene
    let private finish = Finish >> GamePlayScene

    let private playerMove state position = 
        let (Vector(x,y)) = position
        let (Vector(xo, yo)) = state.Origin
        let column = (x - xo) / state.Back.CellWidth |> int
        let raw = (y - yo) / state.Back.CellHeight |> int
        let cell = Grid.get state.Grid raw column 

        match cell with
        | Empty _ -> PlayerMoved <| Grid.update state.Grid raw column (Occupied(state.Move, X))
        | _       -> PlayerThinking

    let nextPlayer = function | Player -> AI | AI -> Player

    let init state events = 

        let fonts = events |> List.map (fun event -> match event with | FontLoadedEvent(path, font) -> Some (path, font) | _ -> None) |> List.filter Option.isSome |> List.map Option.get
        let textures = events |> List.map (fun event -> match event with | TextureLoadedEvent(path, texture) -> Some (path, texture) | _ -> None) |> List.filter Option.isSome |> List.map Option.get
        let fonts = state.Fonts@fonts
        let textures = state.Textures@textures

        if fonts |> List.exists (fun (path, _) -> path = "Fonts/H1") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [LoadFontCommand "Fonts/H1"])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/Back") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [LoadTextureCommand "Sprites/Back"])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/X") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [LoadTextureCommand "Sprites/X"])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/O") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [LoadTextureCommand "Sprites/O"])
        else 
            let screenWidth = 1920.0f<pixel>
            let screenHeight = 1080.0f<pixel>

            let back = textures |> List.filter (fun (path, _) -> path = "Sprites/Back") |> List.map (fun (_, font) -> font) |> List.exactlyOne
            let x = textures |> List.filter (fun (path, _) -> path = "Sprites/X") |> List.map (fun (_, font) -> font) |> List.exactlyOne
            let o = textures |> List.filter (fun (path, _) -> path = "Sprites/O") |> List.map (fun (_, font) -> font) |> List.exactlyOne
            let font = fonts |> List.filter (fun (path, _) -> path = "Fonts/H1") |> List.map (fun (_, font) -> font) |> List.exactlyOne

            let (Vector(width, height)) =  Texture.size back
            let position = Vector.init ((screenWidth / 2.0f) - (width / 2.0f)) ((screenHeight / 2.0f) - (height / 2.0f))

            let state =
                { 
                    Content = { X = x; O = o; Font = font; }
                    Grid = initGrid
                    Origin = position
                    Move = Player
                    Back = { Sprite = Sprite(position, back, Vector.init 1.0f 1.0f); CellWidth = width / 3.0f; CellHeight = height / 3.0f }
                    FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, font, Color.red, ""))
                }
            (play state,[])

    let update state events =
        match state with
        | Play state -> 
            let status = 
                match state.Move with
                | Player -> 
                    let back = state.Back.Sprite
                    onMouseButtonEvent 
                        (fun button state position -> button = MouseButton.Left && state = MouseButtonState.Released && Graphics.inBounds position back) 
                        (fun _ _ position -> playerMove state position) 
                        (fun () -> PlayerThinking) 
                        events

                | AI -> 
                    let (raw, column, _) = state.Grid |> List.filter (fun (_, _, cell) -> match cell with | Empty _ -> true | _ -> false) |> List.maxBy (fun (_, _, (Empty value)) -> value)
                    Grid.update state.Grid raw column (Occupied(state.Move, O)) |> PlayerMoved
            
            match status with 
            | PlayerThinking -> (play state, [])
            | PlayerMoved grid -> 
                let player = nextPlayer state.Move
                match grid with 
                | ContinueGame -> (play { state with Grid = grid; Move = player }, [])
                | Tie -> (finish { state with Grid = grid; Move = player; FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "A draw")) }, [])
                | PlayerWon -> (finish { state with Grid = grid; Move = player; FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Victory")) }, [])
                | AiWon -> (finish { state with Grid = grid; Move = player; FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Defeat")); }, [])

        | Finish state -> 
            onMouseButtonEvent
                (fun _ state _ -> state = MouseButtonState.Released) 
                (fun _ _ _ -> (play { state with Grid = initGrid; }, [])) 
                (fun () -> (finish state, [])) 
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