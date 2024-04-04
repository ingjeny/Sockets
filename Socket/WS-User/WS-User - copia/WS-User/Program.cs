using System.Net.WebSockets;
using System.Text;
var ws = new ClientWebSocket();
string name;
while (true)
{
    Console.Write("Ingrese su nombre: ");
    name = Console.ReadLine();
    break;
}
Console.WriteLine("Conectando al servidor");
await ws.ConnectAsync(new Uri($"ws://localhost:6969/ws?name={name}"), CancellationToken.None);
Console.WriteLine("Conectado!");
var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024 * 4];
    while (true)
    {
        var result = await ws.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);

        if (result.MessageType ==
        WebSocketMessageType.Close)
        {
            break;
        }
        var message = Encoding.UTF8.GetString(
            buffer, 0, result.Count);
        Console.WriteLine(message);
    }
});
var sendTask = Task.Run(async () =>
{
    while (true)
    {
        var Message = Console.ReadLine();

        if (Message == "salio")
        {
            break;
        }
        var bytes = Encoding.UTF8.GetBytes(Message);
        await ws.SendAsync(new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text, true,
            CancellationToken.None);
    }
});
await Task.WhenAny(sendTask, receiveTask);
if (ws.State != WebSocketState.Closed)
{
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure
        , "Closing", CancellationToken.None);
}
await Task.WhenAll(sendTask, receiveTask);