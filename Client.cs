
using System.Net.Sockets;
class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    TcpClient client;
    ServerObject server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        var stream = client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        try
        {
            string? userName = await Reader.ReadLineAsync();
            string? message = $"{userName} connected";
            Console.WriteLine(message);
            while (true)
            {
                try
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    message = $"\t {message}";
                    Console.WriteLine(message);
                }
                catch
                {
                    message = $"{userName} disconnected";
                    Console.WriteLine(message);
                    break;
                }
            }
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
        finally { server.RemoveConnection(Id); }
    }
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}