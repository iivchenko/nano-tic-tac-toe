namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content
open Flame.Input

type Sym = | X | O
type Player = | AI of Sym | Human of Sym

type Cell =
| Empty of value: int
| Occupied of Player

type GamePlaySceneContent =
    { X: Texture
      O: Texture 
      Font: Font }

type GamePlayBackInfo = 
    { Sprite: Graphics
      CellWidth: float32<pixel>
      CellHeight: float32<pixel> }

type GamePlayStatus = | Playing | Finish of message: Graphics

type GamePlaySceneState = 
    { Content: GamePlaySceneContent
      Origin: Vector<pixel>
      Grid: (int * int * Cell) list
      Back: GamePlayBackInfo
      Status: GamePlayStatus
      Move: Player 
      PreivosMouseLeftButton: MouseButtonState}

type GamePlayEvent =
    | None of state:GamePlaySceneState
    | Exit

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

    let private filterMouse input = 
        match input with 
        | Mouse _ -> true
        | _       -> false

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

    let init api settings = 
        let back = api.LoadTexture("Sprites/Back")
        let x = api.LoadTexture("Sprites/X")
        let o = api.LoadTexture("Sprites/O")
        let font = api.LoadFont("Fonts/H1")
        let (Vector(width, height)) =  Texture.size back
        let position = Vector.init ((settings.ScreenWidth / 2.0f) - (width / 2.0f)) ((settings.ScreenHeight / 2.0f) - (height / 2.0f))

        { 
            Content = { X = x; O = o; Font = font; }
            Grid = grid
            Origin = position
            Status = Playing
            Move = Human(X)
            Back = { Sprite = Sprite(position, back, Vector.init 1.0f 1.0f); CellWidth = width / 3.0f; CellHeight = height / 3.0f }
            PreivosMouseLeftButton = MouseButtonState.Released
        }

    let update state inputs =
        match state.Status with
        | Playing -> 
            match state.Move with
            | Human(sym) -> 
                let input = inputs |> List.filter filterMouse |> List.exactlyOne
                match input with
                | Mouse mouse when mouse.LeftButton <> state.PreivosMouseLeftButton -> 
                    if mouse.LeftButton = MouseButtonState.Released && Graphics.inBounds mouse.Position state.Back.Sprite
                        then 
                            let (Vector(x,y)) = mouse.Position
                            let (Vector(xo, yo)) = state.Origin
                            let column = (x - xo) / state.Back.CellWidth |> int
                            let raw = (y - yo) / state.Back.CellHeight |> int
                            let cell = Grid.get state.Grid raw column 

                            match cell with
                            | Empty _ -> 
                                let grid = Grid.update state.Grid raw column (Occupied(state.Move))                                

                                if [0..2] |> List.exists (winHumanRaw grid) || [0..2] |> List.exists (winHumanColumn grid) || (winHumanMainDiagnal grid) || (winHumanSecondDiagnal grid)
                                    then { state with PreivosMouseLeftButton = mouse.LeftButton; Grid = grid; Move = AI(O); Status = Finish (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Victory")); } 
                                    elif grid |> List.forall (fun (_, _, cell) -> match cell with | Occupied _ -> true | _ -> false) 
                                        then { state with PreivosMouseLeftButton = mouse.LeftButton; Grid = grid; Move = AI(O); Status = Finish (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "A draw")); } 
                                    else { state with PreivosMouseLeftButton = mouse.LeftButton; Grid = grid; Move = AI(O) }

                            | _ -> { state with PreivosMouseLeftButton = mouse.LeftButton; }
                        else 
                            { state with PreivosMouseLeftButton = mouse.LeftButton }
                | _ -> state

            | AI(sym) -> 
                let (raw, column, _) = state.Grid |> List.filter (fun (_, _, cell) -> match cell with | Empty _ -> true | _ -> false) |> List.maxBy (fun (_, _, (Empty value)) -> value)
                let grid = Grid.update state.Grid raw column (Occupied(state.Move))
                
                if [0..2] |> List.exists (winAiRaw grid) || [0..2] |> List.exists (winAiColumn grid) || (winAiMainDiagnal grid) || (winAiSecondDiagnal grid)
                    then { state with Grid = grid; Move = Human(X); Status = Finish (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "Defeat")); } 
                    elif grid |> List.forall (fun (_, _, cell) -> match cell with | Occupied _ -> true | _ -> false) 
                        then { state with Grid = grid; Move = Human(X); Status = Finish (Text(Vector.init 0.0f<pixel> 0.0f<pixel>, state.Content.Font, Color.red, "A draw")); } 
                    else { state with Grid = grid; Move = Human(X) }

        | Finish _ -> 
            let input = inputs |> List.filter filterMouse |> List.exactlyOne
            match input with
            | Mouse mouse when mouse.LeftButton <> state.PreivosMouseLeftButton -> 
                if mouse.LeftButton = MouseButtonState.Released
                    then { state with PreivosMouseLeftButton = mouse.LeftButton; Grid = grid; Status = Playing; }
                    else { state with PreivosMouseLeftButton = mouse.LeftButton } 
            | _ -> state

    let draw state = 
        let grid = state.Grid
                    |> List.map (fun (raw, column, cell) -> map (float32 raw) (float32 column) cell state.Content.Font state.Content.X state.Content.O state.Origin state.Back.CellWidth state.Back.CellHeight)
                    |> Seq.cast<Graphics>
                    |> Seq.toList
                    |> Graphics
        match state.Status with
        | Playing -> Graphics(state.Back.Sprite::grid::[])
        | Finish message -> Graphics(state.Back.Sprite::grid::message::[])