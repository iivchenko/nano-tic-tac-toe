namespace NanoTicTacToe.Game

open Flame
open Flame.Graphics
open Flame.Content

type MainMenuSceneState = 
    { Header: Graphics
      Start:  Graphics
      Exit:   Graphics }

type MainMenuInitState = 
    { Resources: (string * Font) list }

type MainMenuEvent =
    | Continue of state: MainMenuSceneState
    | StartGame
    | Exit

type GamePlayInitState = 
    { Textures: (string * Texture) list
      Fonts: (string * Font) list }

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
    
type GamePlaySceneState = 
    { Content: GamePlaySceneContent
      Origin: Vector<pixel>
      Grid: (int * int * Cell) list
      Back: GamePlayBackInfo
      Move: Player 
      FinishMessage: Graphics }
    
type GamePlayEvent =
    | Continue of state:GamePlaySceneState
    | Exit

type GameState = 
    | InitMainMenu of state: MainMenuInitState
    | MainMenuScene of state: MainMenuSceneState
    | InitGamePlay of state: GamePlayInitState
    | GamePlayScene of state: GamePlaySceneState
    | GamePlaySceneFinish of state: GamePlaySceneState