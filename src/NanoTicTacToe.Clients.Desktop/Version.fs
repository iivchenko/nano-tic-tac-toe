module NanoTicTacToe.Clients.Desktop.Version

open Microsoft.Extensions.FileProviders
open System.Reflection
open System.IO

let value = 
    let assembly = Assembly.GetExecutingAssembly()
    let embeddedProvider = new EmbeddedFileProvider(assembly, "NanoTicTacToe.Clients.Desktop")
    use stream = embeddedProvider.GetFileInfo(".version").CreateReadStream()
    use reader = new StreamReader(stream)
    reader.ReadLine()
