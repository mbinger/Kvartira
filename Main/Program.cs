using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        public static void Main(string[] args)
        {
            var appConfig = new AppConfig
            {
                DumpFolder = @"c:\a\dump"
            };
            var log = new Log(appConfig);

            var downloader = new BrowserDownloader(log, appConfig);

            var url1 = "https://www.immobilienscout24.de/expose/130410164";

            var response1 = downloader.GetAsync(url1, false, "ImmoScout24 Treptow-Köpenick", true).GetAwaiter().GetResult();
            downloader.Delay().GetAwaiter().GetResult();
            if (response1.Content.Contains("Ich bin kein Roboter"))
            {
                response1 = downloader.GetAsync(url1, true, "ImmoScout24 Treptow-Köpenick", true).GetAwaiter().GetResult();
                downloader.Delay().GetAwaiter().GetResult();
            }
            File.WriteAllText(@"c:\\a\\1.htm", response1.Content);
        }
    }
}
