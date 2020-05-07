namespace Flame.Input

open Microsoft.Xna.Framework.Input

[<RequireQualifiedAccess>]
type MouseButtonState = 
    | Released
    | Pressed

[<RequireQualifiedAccess>]
type MouseButton = 
    | Left
    | Middle
    | Right

module internal MouseInput =

    let internal toButtonState (button: ButtonState) = 
        match button with 
        | ButtonState.Released -> MouseButtonState.Released
        | ButtonState.Pressed -> MouseButtonState.Pressed
        | _ -> raise (new exn((sprintf "Unknown MonoGame mouser button state: %s" (button.ToString()))))