using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common
{
    public class HttpDownloader: IDownloader
    {
        private readonly Log log;
        private readonly AppConfig appConfig;
        private string userAgent;
        private CookieContainer cookieContainer;
        private Random random;

        public HttpDownloader(Log log, AppConfig appConfig)
        {
            this.log = log;
            this.appConfig = appConfig;
            random = new Random();
        }

        private void Setup()
        {
            cookieContainer ??= new CookieContainer();
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

        public async Task<Response> GetAsync(string url, string description)
        {
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
                await log.LogAsync($"Error downloading content from {url} description {description}: \n{ex}");
                result.Exception = ex.ToString();
            }

            await DumpDownloader.WriteDumpAsync(appConfig, log, result, description);

            return result;
        }
    }
}
