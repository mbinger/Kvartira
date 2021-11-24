using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Proxy
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 4502);

        const int BUFFER_SIZE = 4096;

        static void Main(string[] args)
        {
            listener.Start();
            new Task(() => {
                // Accept clients.
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    new Task(() => {
                        // Handle this client.
                        var clientStream = client.GetStream();
                        TcpClient server = new TcpClient("18.66.2.16", 443);
                        var serverStream = server.GetStream();
                        new Task(() => {
                            byte[] message = new byte[BUFFER_SIZE];
                            int clientBytes;
                            while (true)
                            {
                                try
                                {
                                    clientBytes = clientStream.Read(message, 0, BUFFER_SIZE);
                                }
                                catch
                                {
                                    // Socket error - exit loop.  Client will have to reconnect.
                                    break;
                                }
                                if (clientBytes == 0)
                                {
                                    // Client disconnected.
                                    break;
                                }
                                serverStream.Write(message, 0, clientBytes);
                            }
                            client.Close();
                        }).Start();
                        new Task(() => {
                            byte[] message = new byte[BUFFER_SIZE];
                            int serverBytes;
                            while (true)
                            {
                                try
                                {
                                    serverBytes = serverStream.Read(message, 0, BUFFER_SIZE);
                                    clientStream.Write(message, 0, serverBytes);
                                }
                                catch
                                {
                                    // Server socket error - exit loop.  Client will have to reconnect.
                                    break;
                                }
                                if (serverBytes == 0)
                                {
                                    // server disconnected.
                                    break;
                                }
                            }
                        }).Start();
                    }).Start();
                }
            }).Start();
            Console.WriteLine("Server listening on port 4502.  Press enter to exit.");
            Console.ReadLine();
            listener.Stop();
        }
    }
}
