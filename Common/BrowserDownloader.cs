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
        private readonly ILog log;
        private readonly AppConfig appConfig;
        private Random random;

        private static ConcurrentBag<string> requests = new ConcurrentBag<string>();
        private static MemoryCache responses = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });

        private static HttpServer requestListener = null;
        private static TcpListener responseListener = null;
        private static object lockObj = new object();
        private static SemaphoreSlim semaphore = new SemaphoreSlim(0);
        private static Task requestListenerTask;
        private static Task responseListenerTask;
        private static CancellationTokenSource requestListenerCancellation;
        private static CancellationTokenSource responseListenerCancellation;

        public BrowserDownloader(ILog log, AppConfig appConfig)
        {
            this.log = log;
            this.appConfig = appConfig;
            random = new Random();
        }

        public Task Delay()
        {
            var delay = random.Next(1000, 3000);
            return Task.Delay(delay);
        }

        private TimeSpan GetTimeOut = TimeSpan.FromMinutes(5);

        public async Task<Response> GetAsync(string url, bool fromCache, string description, bool deflate)
        {
            var result = await GetAsyncIntern(url, fromCache, description, deflate);

            DumpDownloader.WriteDump(appConfig, log, result, description);

            if (deflate && !string.IsNullOrEmpty(result.Content))
            {
                result.Content = Scanner.DeflateHtml(result.Content);
                DumpDownloader.WriteDump(appConfig, log, result, description + "_deflated");
            }

            return result;
        }

        private async Task<Response> GetAsyncIntern(string url, bool fromCache, string description, bool deflate)
        {
            if (fromCache)
            {
                if (responses.TryGetValue(url, out var content2))
                {
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

            responses.Remove(url);
            requests.Add(url);

            Start();

            var ct = new CancellationTokenSource();
            var timeoutTask = Task.Delay(GetTimeOut, ct.Token);

            while (true)
            {
                if (responses.TryGetValue(url, out var content2))
                {
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
                    requestListenerCancellation = new CancellationTokenSource();
                    requestListenerTask = StartRequestListener();
                }
                if (responseListener == null)
                {
                    responseListenerCancellation = new CancellationTokenSource();
                    responseListenerTask = StartResponseListener();
                }
            }
        }

        const int BUFFER_SIZE = 4096;

        private Task StartRequestListener()
        {
            return Task.Run(() =>
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
                            return new HttpResponse
                            {
                                StatusCode = "200",
                                ContentAsUTF8 = url,
                                Headers = new Dictionary<string, string>
                                {
                                    { "Content-Type", "text/plain"}
                                }
                            };
                        }
                        else
                        {
                            return new HttpResponse
                            {
                                StatusCode = "200",
                                ContentAsUTF8 = "",
                                Headers = new Dictionary<string, string>
                                {
                                    { "Content-Type", "text/plain"}
                                }
                            };
                        }
                    }
                };
                requestListener = new HttpServer(82, new List<Route> { route });
                requestListener.Listen();
            });
        }

        private Task StartResponseListener()
        {
            responseListener = new TcpListener(IPAddress.Any, 83);
            responseListener.Start();
            return Task.Run(async () =>
            {
                try
                {
                    // Accept clients.
                    while (!responseListenerCancellation.Token.IsCancellationRequested)
                    {
                        var client = await responseListener.AcceptTcpClientAsync();
                        var _ = Task.Run(() =>
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
                                log.Write("Browser download: StartListener\n" + ex.ToString());
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    log.Write("Browser download: StartListener\n" + ex.ToString());
                }
            });
        }

        public void Dispose()
        {
            if (responseListener != null)
            {
                responseListenerCancellation.Cancel();
                responseListener.Stop();
                responseListener = null;
            }
            if (requestListener != null)
            {
                requestListenerCancellation.Cancel();
                requestListener.Stop();
                requestListener = null;
            }
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
