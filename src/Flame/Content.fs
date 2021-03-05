namespace Flame.Content

open Microsoft.Xna.Framework.Graphics

open Flame

type Texture = | Texture of id: string * width : float32<pixel> * height: float32<pixel>
type Font = | Font of font: SpriteFont
type Sound = | Sound of id: string
type Song = | Song of id: string

module Font = 

    // TODO: Refactor MonoGame dependencie!
    let private toPixelVector (v: Microsoft.Xna.Framework.Vector2) = Vector.init (v.X * 1.0f<pixel>) (v.Y * 1.0f<pixel>)

    let length (Font(font)) (text: string) = font.MeasureString text |> toPixelVector


