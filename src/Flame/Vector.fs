namespace Flame

type Vector<[<Measure>] 'u> =
    | Vector of x: float32<'u> * y: float32<'u>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Vector = 
    let init x y = Vector(x, y)


