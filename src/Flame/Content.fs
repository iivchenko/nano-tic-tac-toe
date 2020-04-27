namespace Flame.Content

open Microsoft.Xna.Framework.Graphics

type Font = internal | Font of font: SpriteFont

module Font = 

    let load font = Font font
    let length (Font(font)) (text: string) = font.MeasureString text

type Texture = internal | Texture of Texture2D

module Texture =

    let load texture = Texture texture