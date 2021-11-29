using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common
{
    public class HttpDownloader: IDownloader
    {
        private readonly ILog log;
        private readonly AppConfig appConfig;
        private string userAgent;
        private CookieContainer cookieContainer;
        private Random random;

        public HttpDownloader(ILog log, AppConfig appConfig)
        {
            this.log = log;
            this.appConfig = appConfig;
            random = new Random();
        }

        private void Setup()
        {
            if (cookieContainer == null)
            {
                cookieContainer = new CookieContainer();
            }
            if (string.IsNullOrEmpty(userAgent))
            {
                var count = appConfig.UserAgents.Length;
                if (count == 1)
                {
                    userAgent = appConfig.UserAgents[0];
                }
                else
                {
                    var index = random.Next(0, count - 1);
                    userAgent = appConfig.UserAgents[index];
                }
            }
        }

        public Task Delay()
        {
            var delay = random.Next(1000, 3000);
            return Task.Delay(delay);
        }

        public async Task<Response> GetAsync(string url, bool fromcache, string description, bool deflate)
        {
            if (fromcache)
            {
                throw new Exception("Cache not supported");
            }

            var result = new Response
            {
                Url = url
            };

            try
            {
                Setup();

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookieContainer;
                request.Method = "GET";
                request.UserAgent = userAgent;
                request.ContentType = "text/plain;charset=UTF-8";

                if (!string.IsNullOrEmpty(appConfig.ProxyUrl))
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri(appConfig.ProxyUrl),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = true
                    };
                    request.Proxy = proxy;
                }

                var response = (HttpWebResponse)await request.GetResponseAsync();

                result.HttpStatusCode = (int)response.StatusCode;
                using (var stream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        result.Content = await streamReader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write($"Error downloading content from {url} description {description}: \n{ex}");
                result.Exception = ex.ToString();
            }

            DumpDownloader.WriteDump(appConfig, log, result, description);

            if (deflate && !string.IsNullOrEmpty(result.Content))
            {
                result.Content = Scanner.DeflateHtml(result.Content);
                DumpDownloader.WriteDump(appConfig, log, result, description + "_deflated");
            }

            return result;
        }
    }
}
