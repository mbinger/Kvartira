using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Downloader
    {
        private readonly Log log;
        private readonly AppConfig appConfig;
        private string userAgent;
        private CookieContainer cookieContainer;

        public Downloader(Log log, AppConfig appConfig)
        {
            this.log = log;
            this.appConfig = appConfig;
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
                    var rnd = new Random();
                    var index = rnd.Next(0, count - 1);
                    userAgent = appConfig.UserAgents[index];
                }
            }
        }

        public class Response
        {
            public string Url { get; set; }
            public string Exception { get; set; }
            public int HttpStatusCode { get; set; }
            public string Content { get; set; }
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

            await Dump(result, description);

            return result;
        }

        private async Task Dump(Response resp, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(appConfig.DumpFolder))
                {
                    return;
                }

                var fileName = $"{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}_{description}.txt";
                var path = Path.Combine(appConfig.DumpFolder, fileName);

                if (!Directory.Exists(appConfig.DumpFolder))
                {
                    Directory.CreateDirectory(appConfig.DumpFolder);
                }

                var content = new StringBuilder();
                content.AppendLine(resp.Url);
                content.AppendLine(resp.HttpStatusCode.ToString());
                content.AppendLine(resp.Exception?.Replace("\n", "")?.Replace("\r", ""));
                content.AppendLine(resp.Content);

                await File.WriteAllTextAsync(path, content.ToString());
            }
            catch (Exception ex)
            {
                await log.LogAsync($"Error dumping content\n {ex}");
            }
        }
    }
}
