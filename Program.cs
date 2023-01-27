
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Security.Cryptography;

[DllImport("kernel32.dll")]
static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

ShowWindow(GetConsoleWindow(), 0);

string host = "127.0.0.1";
int port = 8888;
using TcpClient client = new TcpClient();
StreamReader? Reader = null;
StreamWriter? Writer = null;

try
{
    client.Connect(host, port);
    Reader = new StreamReader(client.GetStream());
    Writer = new StreamWriter(client.GetStream());
    if (Writer is null || Reader is null) return;
    await SendMessageAsync(Writer);
}
catch (Exception ex) { Console.WriteLine(ex.Message); }

Writer?.Close();
Reader?.Close();

async Task SendMessageAsync(StreamWriter writer)
{
    IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
    IPAddress ipAddress = endPoint.Address;

    await writer.WriteLineAsync(Dns.GetHostEntry(ipAddress).HostName);
    await writer.WriteLineAsync($"DateTime: {DateTime.Now} \n");
    foreach (var item in Process.GetProcesses())
    {
        await writer.WriteLineAsync($"Process {item.ProcessName}\n Id: {item.Id}\n Priority: " +
            $"{item.BasePriority}\n Hash code: {item.GetHashCode()}\n ");
    }
    await writer.FlushAsync();
}
