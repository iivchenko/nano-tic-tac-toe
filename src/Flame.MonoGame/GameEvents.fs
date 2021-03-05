namespace Flame.MonoGame

open Flame
open Flame.Content
open Flame.Input

type GameEvent =
    | ScreenSizeUpdatedEvent of width: int * height: int
    | FontLoadedEvent        of path: string * sprite: Font
    | TextureLoadedEvent     of path: string * texture: Texture
    | SoundLoadedEvent       of path: string * sound: Sound
    | SongLoadedEvent        of path: string * song: Song
    | MouseMovedEvent        of position: Vector<pixel>
    | MouseButtonEvent       of button: MouseButton * state: MouseButtonState * position: Vector<pixel>

