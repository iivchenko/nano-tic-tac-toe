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

    let private grid = 
        [ 
            (0, 0, Empty 3); (0, 1, Empty 2); (0, 2, Empty 3);
            (1, 0, Empty 2); (1, 1, Empty 4); (1, 2, Empty 2);  
            (2, 0, Empty 3); (2, 1, Empty 2); (2, 2, Empty 3);
        ]

    let private mouseClick events = events |> List.tryFind (fun event -> match event with | GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, _)) -> true | _ -> false)

    let private map row column value font xs os (Vector(x, y)) (width:float32<pixel>) (height:float32<pixel>) =
        match value with
        | Empty v -> 
            let (Vector(fW, fH)) = Font.length font (sprintf "%i" v)
            let x = x + width * column + width / 2.0f - fW / 2.0f
            let y = y + height * row + height / 2.0f - fH / 2.0f
            Text(Vector.init x y, font, Color.grey, sprintf "%i" v)
        | Occupied((AI(a))) 
        | Occupied((Human(a))) ->
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

    let private winHumanRaw grid index = Grid.raw grid index |> List.forall(fun (_, _, cell) -> match cell with | Occupied(Human _) -> true | _ -> false)
    let private winHumanColumn grid index = Grid.column grid index |> List.forall(fun (_, _, cell) -> match cell with | Occupied(Human _) -> true | _ -> false)
    let private winHumanMainDiagnal (grid: (int * int * Cell) list) =  (Grid.get grid 0 0)::(Grid.get grid 1 1)::(Grid.get grid 2 2)::[] |> List.forall(fun (cell) -> match cell with | Occupied(Human _) -> true | _ -> false)
    let private winHumanSecondDiagnal (grid: (int * int * Cell) list) =  (Grid.get grid 2 0)::(Grid.get grid 1 1)::(Grid.get grid 0 2)::[] |> List.forall(fun (cell) -> match cell with | Occupied(Human _) -> true | _ -> false)

    let private winAiRaw grid index = Grid.raw grid index |> List.forall(fun (_, _, cell) -> match cell with | Occupied(AI _) -> true | _ -> false)
    let private winAiColumn grid index = Grid.column grid index |> List.forall(fun (_, _, cell) -> match cell with | Occupied(AI _) -> true | _ -> false)
    let private winAiMainDiagnal (grid: (int * int * Cell) list) =  (Grid.get grid 0 0)::(Grid.get grid 1 1)::(Grid.get grid 2 2)::[] |> List.forall(fun (cell) -> match cell with | Occupied(AI _) -> true | _ -> false)
    let private winAiSecondDiagnal (grid: (int * int * Cell) list) =  (Grid.get grid 2 0)::(Grid.get grid 1 1)::(Grid.get grid 0 2)::[] |> List.forall(fun (cell) -> match cell with | Occupied(AI _) -> true | _ -> false)

    let init state events = 

        let fonts = events |> List.map (fun event -> match event with | ContentEvent(FontLoaded(path, font)) -> Some (path, font) | _ -> None) |> List.filter Option.isSome |> List.map Option.get
        let textures = events |> List.map (fun event -> match event with | ContentEvent(TextureLoaded(path, texture)) -> Some (path, texture) | _ -> None) |> List.filter Option.isSome |> List.map Option.get
        let fonts = state.Fonts@fonts
        let textures = state.Textures@textures

        if fonts |> List.exists (fun (path, _) -> path = "Fonts/H1") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [ContentCommand(LoadFont "Fonts/H1")])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/Back") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [ContentCommand(LoadTexture "Sprites/Back")])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/X") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [ContentCommand(LoadTexture "Sprites/X")])
        elif textures |> List.exists (fun (path, _) -> path = "Sprites/O") |> not
            then (InitGamePlay({ state with Fonts = fonts; Textures = textures; }), [ContentCommand(LoadTexture "Sprites/O")])
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
                    Grid = grid
                    Origin = position
                    Move = Human(X)
                    Back = { Sprite = Sprite(position, back, Vector.init 1.0f 1.0f); CellWidth = width / 3.0f; CellHeight = height / 3.0f }
                    FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, font, Color.red, ""))
                }
            (GamePlayScene(Continue(state)),[])

    let update state events =
        match state with
        | Continue state -> 
            match state.Move with
            | Human(sym) -> 
                match mouseClick events with
                | Some(GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, position))) when Graphics.inBounds position state.Back.Sprite -> 
                    let (Vector(x,y)) = position
                    let (Vector(xo, yo)) = state.Origin
                    let column = (x - xo) / state.Back.CellWidth |> int
                    let raw = (y - yo) / state.Back.CellHeight |> int
                    let cell = Grid.get state.Grid raw column 

                    match cell with
                    | Empty _ -> 
                        let grid = Grid.update state.Grid raw column (Occupied(state.Move))                                

                        if [0..2] |> List.exists (winHumanRaw grid) || [0..2] |> List.exists (winHumanColumn grid) || (winHumanMainDiagnal grid) || (winHumanSecondDiagnal grid)
                            then 
                                (GamePlayScene(Finish({ state with Grid = grid; Move = AI(O);FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Victory")) })), [])
                            elif grid |> List.forall (fun (_, _, cell) -> match cell with | Occupied _ -> true | _ -> false) 
                                then 
                                    (GamePlayScene(Finish({ state with Grid = grid; Move = AI(O); FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "A draw")) })), [])
                            else 
                                (GamePlayScene(Continue({ state with Grid = grid; Move = AI(O) })), [])

                    | _ -> (GamePlayScene(Continue(state)), [])
                | _ -> (GamePlayScene(Continue(state)), [])

            | AI(sym) -> 
                let (raw, column, _) = state.Grid |> List.filter (fun (_, _, cell) -> match cell with | Empty _ -> true | _ -> false) |> List.maxBy (fun (_, _, (Empty value)) -> value)
                let grid = Grid.update state.Grid raw column (Occupied(state.Move))
                
                if [0..2] |> List.exists (winAiRaw grid) || [0..2] |> List.exists (winAiColumn grid) || (winAiMainDiagnal grid) || (winAiSecondDiagnal grid)
                    then 
                        (GamePlayScene(Finish({ state with Grid = grid; Move = Human(X); FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Defeat")); })), [])
                    elif grid |> List.forall (fun (_, _, cell) -> match cell with | Occupied _ -> true | _ -> false) 
                        then 
                            (GamePlayScene(Finish ({ state with Grid = grid; Move = Human(X); FinishMessage = (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "A draw")) })), [])
                    else 
                        (GamePlayScene(Continue({ state with Grid = grid; Move = Human(X) })), [])

        | Finish state -> 
            match mouseClick events with
            | Some(GameEvent.Mouse(MouseEvent.Button(Left, MouseButtonState.Released, position))) -> (GamePlayScene(Continue({ state with Grid = grid; })), [])
            | _ -> (GamePlayScene(Finish(state)), [])

    let draw state =
        match state with
        | Continue state -> 
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