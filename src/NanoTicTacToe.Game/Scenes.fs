namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content

type MainMenuSceneState = 
    { Header: Graphics 
      Start:  Graphics 
      Exit:   Graphics }

type MainMenuEvent =
    | Continue of state: MainMenuSceneState
    | StartGame
    | Exit

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
      Move: Player }
    
type GamePlayEvent =
    | Continue of state:GamePlaySceneState
    | Exit

type Scenes = 
    | InitMainMenu
    | MainMenuScene of state: MainMenuSceneState
    | InitGamePlay
    | GamePlayScene of state: GamePlaySceneState
    | Exit