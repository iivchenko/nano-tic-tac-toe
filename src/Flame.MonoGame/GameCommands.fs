namespace Flame.MonoGame

open Flame.Graphics
open Flame.Content

type GameCommand =
    | UpdateScreenSizeCommand of width: int * height: int
    | LoadFontCommand         of path: string
    | LoadTextureCommand      of path: string
    | LoadSoundCommand        of path: string
    | LoadSongCommand         of path: string
    | PlaySoundCommand        of sound: Sound
    | PlaySongCommand         of song: Song
    | DrawCommand             of graphics: Graphics
    | ExitGameCommand