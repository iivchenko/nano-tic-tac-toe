namespace Flame.Content

open Microsoft.Xna.Framework.Graphics

open Flame

type Font = | Font of font: SpriteFont

module Font = 

    // TODO: Refactor MonoGame dependencie!
    let private toPixelVector (v: Microsoft.Xna.Framework.Vector2) = Vector.init (v.X * 1.0f<pixel>) (v.Y * 1.0f<pixel>)

    let length (Font(font)) (text: string) = font.MeasureString text |> toPixelVector

// TODO: Refactor MonoGame dependencie!
type Texture = | Texture of Texture2D

module Texture =

    let size (Texture(texture)) = Vector.init (texture.Width |> float32 |> (*) 1.0f<pixel>) (texture.Height |> float32 |> (*) 1.0f<pixel>)

// TODO: Refactor MonoGame dependencie!
type Sound = | Sound of sound: Microsoft.Xna.Framework.Audio.SoundEffect
type Song = | Song of song: Microsoft.Xna.Framework.Media.Song  
