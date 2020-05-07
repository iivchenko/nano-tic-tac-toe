namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content

type GamePlayInitState = 
    { FirstRun: bool }

type GamePlaySceneContent =
    { X: Texture
      O: Texture 
      Font: Font 
      MoveSound: Sound }

type Sym = | X | O
type Player = | AI | Player

    
type Cell =
    | Empty of value: int
    | Occupied of player: Player * sym: Sym

type PlayerActionStatus = | PlayerMoved of grid: (int * int * Cell) list| PlayerThinking
    
type GamePlayBackInfo = 
    { Sprite: Graphics
      CellWidth: float32<pixel>
      CellHeight: float32<pixel> }
    
type GamePlayState = 
    { Content: GamePlaySceneContent
      Origin: Vector<pixel>
      Grid: (int * int * Cell) list
      Back: GamePlayBackInfo
      CurrentPlayer: Player
      FinishMessage: Graphics 
      MoveDelay: float32<second>}
    
type GamePlaySceneState =
    | Play   of state: GamePlayState
    | Finish of state: GamePlayState