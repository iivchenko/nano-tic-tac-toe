open NanoTicTacToe.Clients.Desktop

[<EntryPoint>]
let main _ =
    let game = new TheGame()
    game.Run()

    0