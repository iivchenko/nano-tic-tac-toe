namespace Flame.Graphics

open Microsoft.Xna.Framework.Graphics

open Flame
open Flame.Content

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
    | Sprite   of position: Vector<pixel>
    | Text     of position: Vector<pixel> * font: Font * color: Color * text: string
    | Graphics of graphics: Graphics list

module Graphics =

    let toXna (Color(r, g, b, a)) =  Microsoft.Xna.Framework.Color(r, g, b, a)
    
    let rec private drawIn (spriteBatch: SpriteBatch) (graphics: Graphics) =
        match graphics with 
        | Sprite(position) -> ()
        | Text(position, (Font(xnaFont)), color, text) ->
            spriteBatch.DrawString(xnaFont, text, position |> Utils.toVector2, toXna color)
        | Graphics(graphics) -> graphics |>  List.iter (fun x -> drawIn spriteBatch x)

    let internal draw (spriteBatch: SpriteBatch) (graphics: Graphics) = 

        spriteBatch.Begin()
        drawIn spriteBatch graphics
        spriteBatch.End()

    let inBounds (Vector(x, y)) element = 
        match element with 
        | Text(Vector(tx, ty), font, _, text) -> 
            let (Vector(width, heigth)) = Font.length font text

            x > tx && y > ty && x < tx + width && y < ty + heigth
        | _ -> false