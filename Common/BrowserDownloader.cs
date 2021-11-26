using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class BrowserDownloader : IDownloader, IDisposable
    {
        private static TcpListener responseListener = null;
        private static object lockObj = new object();

        public BrowserDownloader()
        {
        }

        public Task Delay()
        {
            //todo:
            return Task.CompletedTask;
        }

        public async Task<Response> GetAsync(string url, string description, bool deflate)
        {
            Start();

            await Task.Delay(int.MaxValue);
            return new Response();
        }

        private void Start()
        {
            lock (lockObj)
            {
                if (responseListener == null)
                {
                    StartListener();
                }
            }
        }

        private void StartListener()
        {
            var i = 0;
            const int BUFFER_SIZE = 4096;
            responseListener = new TcpListener(IPAddress.Any, 82);
            responseListener.Start();
            new Task(async () => 
            {
                try
                {
                    // Accept clients.
                    while (true)
                    {
                        var client = await responseListener.AcceptTcpClientAsync();
                        new Task(() =>
                        {
                            try
                            {
                                using (var ms = new MemoryStream())
                                {
                                    var clientStream = client.GetStream();
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
                                        ms.Write(message, 0, clientBytes);
                                        if (clientBytes < BUFFER_SIZE)
                                        {
                                            break;
                                        }
                                    }

                                    client.Close();
                                    var msg = Encoding.UTF8.GetString(ms.ToArray());
                                    var pagePayload = Parse(msg);

                                    if (pagePayload != null)
                                    {
                                        var content = new StringBuilder();
                                        content.AppendLine($"<!--{pagePayload.Url}-->");
                                        content.AppendLine($"<!--200-->");
                                        content.AppendLine($"<!---->");
                                        content.AppendLine(pagePayload.Document);

                                        Interlocked.Increment(ref i);
                                        var path = $@"c:\\a\\{i}.txt";
                                        File.WriteAllText(path, content.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }).Start();
                    }
                }
                catch (Exception ex)
                {

                }
            }).Start();
        }

        private void StartZombieBrowser()
        {

        }

        private async Task ListenResponseAsync()
        {

        }

        public void Dispose()
        {
            responseListener?.Stop();            responseListener = null;        }

        private InterceptedPagePaload Parse(string postContent)
        {
            if (string.IsNullOrEmpty(postContent))
            {
                return null;
            }
            var strings = postContent.Split('\n');
            if (strings.Length == 0)
            {
                return null;
            }

            foreach (var str in strings)
            {
                if (str.StartsWith("{"))
                {
                    try
                    {
                        var payload = JsonConvert.DeserializeObject<InterceptedPagePaload>(str);
                        return payload;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return null;
        }
    }

    public class InterceptedPagePaload
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("document")]
        public string Document { get; set; }
    }
}
