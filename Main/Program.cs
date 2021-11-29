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
            var downloader = new BrowserDownloader();
            var url1 = "https://www.immobilienscout24.de/Suche/de/berlin/berlin/treptow-koepenick/wohnung-mit-balkon-mieten?haspromotion=false&numberofrooms=3.0-&enteredFrom=result_list";

            var url2 = "https://www.immobilienscout24.de/expose/130468093";

            var response1 = downloader.GetAsync(url1, false, "ImmoScout24 Treptow-Köpenick", true).GetAwaiter().GetResult();

            File.WriteAllText(@"c:\\a\\1.htm", response1.Content);


            var response2 = downloader.GetAsync(url2, false, "ImmoScout24 Treptow-Köpenick", true).GetAwaiter().GetResult();

            File.WriteAllText(@"c:\\a\\2.htm", response2.Content);
        }
    }
}
