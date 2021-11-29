using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proxy
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 4502);

        const int BUFFER_SIZE = 4096;

        static void Main(string[] args)
        {
            var sem = new SemaphoreSlim(0);

            new Task(async () =>
            {
                while (true)
                {
                    Console.WriteLine("Waiting...!");
                    await sem.WaitAsync();
                    Console.WriteLine("Semaphore!");
                }
            }).Start();

            new Task(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);
                    sem.Release();
                }
            }).Start();



            Console.ReadLine();





            int i = 0;
            listener.Start();
            new Task(() => {
                // Accept clients.
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    new Task(() => {
                    // Handle this client.
                    var clientStream = client.GetStream();
                    TcpClient server = new TcpClient("webproxy-se.corp.vattenfall.com", 8080);
                    var serverStream = server.GetStream();
                    i++;
                    var fileName = "";
                    if (i < 10)
                    {
                            fileName = "00" + i.ToString();
                    }
                    else if (i < 100)
                    {
                            fileName = "0" + i.ToString();
                    }
                    else
                    {
                     fileName = i.ToString();
                    }

                        var file = File.OpenWrite($"c:\\a\\{fileName}.txt");
                        new Task(() => 
                        {
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
                                file.Write(message, 0, clientBytes);
                             }
                            client.Close();
                        }).Start();
                        new Task(() => 
                        {
                            var bytes = Encoding.UTF8.GetBytes("<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
                            file.Write(bytes, 0, bytes.Length);
                            byte[] message = new byte[BUFFER_SIZE];
                            int serverBytes;
                            while (true)
                            {
                                try
                                {
                                    serverBytes = serverStream.Read(message, 0, BUFFER_SIZE);
                                    clientStream.Write(message, 0, serverBytes);
                                    file.Write(message, 0, serverBytes);
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
                            file.Close();
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
