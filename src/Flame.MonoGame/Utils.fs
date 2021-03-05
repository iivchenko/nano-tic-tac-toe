module internal Flame.MonoGame.Utils

open Flame

let toVector (v: Microsoft.Xna.Framework.Vector2) = Vector.init v.X v.Y

let toPixelVector (v: Microsoft.Xna.Framework.Vector2) = Vector.init (v.X * 1.0f<pixel>) (v.Y * 1.0f<pixel>)

let pointToPixelVector (p: Microsoft.Xna.Framework.Point) = Vector.init ((float32 p.X) * 1.0f<pixel>) ((float32 p.Y) * 1.0f<pixel>)

let toVector2 (Vector(x, y)) = Microsoft.Xna.Framework.Vector2(float32 x, float32 y)