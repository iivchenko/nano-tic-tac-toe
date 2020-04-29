namespace Flame.Input

open Microsoft.Xna.Framework.Input

open Flame

[<RequireQualifiedAccess>]
type MouseButtonState = 
    | Released
    | Pressed

type MouseState =
    {
        Position: Vector<pixel>
        LeftButton: MouseButtonState
        RightButton: MouseButtonState
        MiddleButton: MouseButtonState
    }

module Mouse =

    let private toButtonState (button: ButtonState) = 
        match button with 
        | ButtonState.Released -> MouseButtonState.Released
        | ButtonState.Pressed -> MouseButtonState.Pressed
        | _ -> raise (new exn((sprintf "Unknown MonoGame mouser button state: %s" (button.ToString()))))

    let state () = 
        let state = Mouse.GetState()
        {
            Position = Utils.pointToPixelVector state.Position
            LeftButton = toButtonState state.LeftButton 
            RightButton = toButtonState state.RightButton
            MiddleButton = toButtonState state.MiddleButton
        }
