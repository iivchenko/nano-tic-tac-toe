namespace Flame.MonoGame.Input

open Microsoft.Xna.Framework.Input
open Flame.Input

module MonoGameMouseInput =

    let toButtonState (button: ButtonState) = 
        match button with 
        | ButtonState.Released -> MouseButtonState.Released
        | ButtonState.Pressed -> MouseButtonState.Pressed
        | _ -> raise (new exn((sprintf "Unknown MonoGame mouser button state: %s" (button.ToString()))))