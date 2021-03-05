namespace Flame.MonoGame

open System
open System.Collections.Generic

open Flame
open Flame.Content
open Flame.Graphics

module Graphics =

    let private toXna (Color(r, g, b, a)) =  Microsoft.Xna.Framework.Color(r, g, b, a)
    
    let rec private drawIn (spriteBatch: XnaSpriteBatch) (graphics: Graphics) (contnet: IDictionary<string, System.Object>) =
        match graphics with 
        | Sprite(position, (Texture(id, _, _)), scale) ->
            let xnaTexture = contnet.[id] :?> XnaTexture
            spriteBatch.Draw(xnaTexture, position |> Utils.toVector2, Nullable<Microsoft.Xna.Framework.Rectangle>(), toXna Color.white, 0.0f, Utils.toVector2 (Vector.init 0.0f 0.0f), Utils.toVector2 scale, XnaSpriteEffects.None, 0.0f)
        | Text(position, (Font(xnaFont)), color, text) ->
            spriteBatch.DrawString(xnaFont, text, position |> Flame.MonoGame.Utils.toVector2, toXna color)
        | Graphics(graphics) -> graphics |>  List.iter (fun x -> drawIn spriteBatch x contnet)

    let public draw (spriteBatch: XnaSpriteBatch) (graphics: Graphics) (content: IDictionary<string, System.Object>) = 

        spriteBatch.Begin()
        drawIn spriteBatch graphics content
        spriteBatch.End()

    let inBounds (Vector(x, y)) element = 
        match element with 
        | Text(Vector(tx, ty), font, _, text) -> 
            let (Vector(width, height)) = Font.length font text

            x > tx && y > ty && x < tx + width && y < ty + height
        | Sprite(Vector(sx, sy), texture, _) -> 
            let (Texture(_, width, height)) = texture

            x > sx && y > sy && x < sx + width && y < sy + height
        | _ -> false