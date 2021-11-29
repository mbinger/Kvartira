using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
        private static ConcurrentBag<string> requests = new ConcurrentBag<string>();
        private static MemoryCache responses = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });

        private static HttpServer requestListener = null;
        private static TcpListener responseListener = null;
        private static object lockObj = new object();
        private static SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public BrowserDownloader()
        {
        }

        public Task Delay()
        {
            //todo:
            return Task.CompletedTask;
        }

        private TimeSpan GetTimeOut = TimeSpan.FromMinutes(5);

        public async Task<Response> GetAsync(string url, bool fromCache, string description, bool deflate)
        {
            if (fromCache)
            {
                if (responses.TryGetValue(url, out var content2))
                {
                    responses.Remove(url);

                    return new Response
                    {
                        HttpStatusCode = 200,
                        Content = (string)content2,
                        Url = url
                    };
                }
                else
                {
                    return null;
                }
            }

            requests.Add(url);

            Start();

            var ct = new CancellationTokenSource();
            var timeoutTask = Task.Delay(GetTimeOut, ct.Token);

            while (true)
            {
                if (responses.TryGetValue(url, out var content2))
                {
                    responses.Remove(url);
                    ct.Cancel();

                    return new Response
                    {
                        HttpStatusCode = 200,
                        Content = (string)content2,
                        Url = url
                    };
                }

                if (timeoutTask.IsCompleted)
                {
                    return new Response
                    {
                        Exception = "Timeout"
                    };
                }

                await semaphore.WaitAsync(TimeSpan.FromSeconds(1));
            }
        }

        private void Start()
        {
            lock (lockObj)
            {
                if (requestListener == null)
                {
                    StartRequestListener();
                }
                if (responseListener == null)
                {
                    StartResponseListener();
                }
            }
        }

        const int BUFFER_SIZE = 4096;

        private void StartRequestListener()
        {
            Task.Run(() =>
            {
                var route = new Route
                {
                    Method = "GET",
                    Name = "index",
                    UrlRegex = "",
                    Callable = (HttpRequest req) =>
                    {
                        var requestExits = requests.TryTake(out var url);

                        if (requestExits)
                        {

                        }

                        return new HttpResponse
                        {
                            StatusCode = "200",
                            ContentAsUTF8 = requestExits
                                ? url
                                : "",
                            Headers = new Dictionary<string, string>
                            {
                            { "Content-Type", "text/plain"}
                            }
                        };
                    }
                };
                requestListener = new HttpServer(82, new List<Route> { route });
                requestListener.Listen();
            });
        }

        private void StartResponseListener()
        {
            responseListener = new TcpListener(IPAddress.Any, 83);
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
                                        responses.Set(pagePayload.Url, pagePayload.Document, TimeSpan.FromMinutes(5));
                                        semaphore.Release();
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

        public void Dispose()
        {
        }

        private InterceptedPagePayload Parse(string postContent)
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
                        var payload = JsonConvert.DeserializeObject<InterceptedPagePayload>(str);
                        return payload;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return null;
        }

        public class InterceptedPagePayload
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("document")]
            public string Document { get; set; }
        }
    }
}
