namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content

type GamePlayInitState = 
    { Textures: (string * Texture) list
      Fonts: (string * Font) list }

type GamePlaySceneContent =
    { X: Texture
      O: Texture 
      Font: Font }

type Sym = | X | O
type Player = | AI of Sym | Human of Sym
    
type Cell =
    | Empty of value: int
    | Occupied of Player
    
type GamePlayBackInfo = 
    { Sprite: Graphics
      CellWidth: float32<pixel>
      CellHeight: float32<pixel> }
    
type GamePlayState = 
    { Content: GamePlaySceneContent
      Origin: Vector<pixel>
      Grid: (int * int * Cell) list
      Back: GamePlayBackInfo
      Move: Player 
      FinishMessage: Graphics }
    
type GamePlaySceneState =
    | Continue of state:GamePlayState
    | Finish of state:GamePlayState