using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data.Models;

using var context = new AppDbContext();
const int LIMIT = 5;
int port = 13000;
var localAddress = IPAddress.Parse("127.0.0.1");
var tcpListener = new TcpListener(localAddress, port);
tcpListener.Start();
Console.WriteLine($"Server mounted, listening to port {port}");

// accept and serve more than one client
for (int i = 0; i < LIMIT; i++)
{
    Thread t = new(new ThreadStart(Service));
    t.Start();
}

Service();

void Service()
{
    while (true)
    {
        Socket sock = tcpListener.AcceptSocket();
        Console.WriteLine($"Connected {sock.RemoteEndPoint}");

        try
        {
            var networkStream = new NetworkStream(sock);
            var streamReader = new StreamReader(networkStream);
            var streamWriter = new StreamWriter(networkStream)
            {
                AutoFlush = true
            };

            while (true)
            {
                string? message = streamReader.ReadLine();
                Console.WriteLine(message);

                if (string.IsNullOrEmpty(message) || message.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else if (message.Equals("LIST", StringComparison.OrdinalIgnoreCase))
                {
                    var announcements = context.Announcements.ToList();
                    var dataForResponse = announcements.Select(x => x.Title).ToArray();

                    var sb = new StringBuilder();

                    for (int i = 0; i < dataForResponse.Length; i++)
                    {
                        sb.Append(dataForResponse[i] + ";");
                    }

                    string responseMessage = sb.ToString();
                    Console.WriteLine(responseMessage);
                    streamWriter.WriteLine(responseMessage);
                }
                else
                {
                    try
                    {
                        var announcement = new Announcement { Title = message };
                        context.Add(announcement);
                        context.SaveChanges();

                        Console.WriteLine($"Message added: \"{message}\"");
                        streamWriter.WriteLine($"Message added: \"{message}\"");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine(ex.Message);
                        streamWriter.WriteLine("ERROR: This announcement already exist!");
                    }
                }
            }

            networkStream.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }

        Console.WriteLine($"Disconnected:: {sock.RemoteEndPoint}");
        sock.Close();
    }
}
