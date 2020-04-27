namespace Flame.Graphics

open Microsoft.Xna.Framework.Graphics

open Flame

type Color = Color of red: byte * green: byte * blue: byte * alpha: byte 

module Color =
    let white =      Color (255uy, 255uy, 255uy, 255uy)
    let red =        Color (255uy, 0uy,   0uy,   255uy)
    let grey =       Color (128uy, 128uy, 128uy, 255uy)
    let coral =      Color (255uy, 127uy, 80uy,  255uy)
    let aquamarine = Color (127uy, 255uy, 212uy, 255uy)
    let black =      Color (0uy,   0uy,   0uy,   255uy)
    let blue =       Color (0uy,   0uy,   255uy, 255uy)

type Graphics =
    | Sprite of position: Vector<pixel>
    | Text of position: Vector<pixel>
    | Graphics of origin: Vector<pixel> * graphics: Graphics list

module Graphics =

    let rec private drawIn (spriteBatch: SpriteBatch) (graphics: Graphics) =
        match graphics with 
        | Sprite(position) -> ()
        | Text(position) -> ()
        | Graphics(origin, graphics)-> ()

    let draw = drawIn