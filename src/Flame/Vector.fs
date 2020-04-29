namespace Flame

type Vector<[<Measure>] 'u> =
    | Vector of x: float32<'u> * y: float32<'u>
    static member (+) ((Vector(x1, y1)), (Vector(x2, y2))) = Vector(x1 + x2, y1 + y2)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector = 
    let init x y = Vector(x, y)


