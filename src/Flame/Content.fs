namespace Flame.Content

open Microsoft.Xna.Framework.Graphics

open Flame

type Font = internal | Font of font: SpriteFont

module Font = 

    let length (Font(font)) (text: string) = font.MeasureString text |> Utils.toPixelVector

type Texture = internal | Texture of Texture2D

module Texture =

    let size (Texture(texture)) = Vector.init (texture.Width |> float32 |> (*) 1.0f<pixel>) (texture.Height |> float32 |> (*) 1.0f<pixel>)

type Sound = internal | Sound of sound: Microsoft.Xna.Framework.Audio.SoundEffect
type Song = internal | Song of song: Microsoft.Xna.Framework.Media.Song  
