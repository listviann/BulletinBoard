using System.Net.Sockets;
using System.Text.RegularExpressions;

int port = 13000;
string serverAddress = "127.0.0.1";

if (args.Length > 0)
    serverAddress = args[0];

var tcpClient = new TcpClient(serverAddress, port);

try
{
    var stream = tcpClient.GetStream();
    var streamReader = new StreamReader(stream);
    var streamWriter = new StreamWriter(stream)
    {
        AutoFlush = true
    };

    while (true)
    {
        Console.WriteLine("Type any word to set and add a title for a new announcement\nType LIST to get all announcements\nType EXIT or leave line empty to close");
        string? message = Console.ReadLine();
        streamWriter.WriteLine(message);

        if (string.IsNullOrEmpty(message) || message.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }
        else
        {
            var response = streamReader.ReadLine();
            Console.WriteLine(response);
        }
    }

    stream.Close();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}
finally
{
    tcpClient.Close();
}