using Common;
using Data;
using Newtonsoft.Json;
using Providers.ImmobilienScout24;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var program = new Program();

            await program.Work();

            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
        }

        public async Task Work()
        {
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("Config.json"));
            WohnungDb.AppConfig = appConfig;

            using (var context = new WohnungDb())
            {
                await context.Database.EnsureCreatedAsync();
            }

            var log = new Log(appConfig);

            var downloader = new HttpDownloader(log, appConfig);

            var provider = new ImmobilienScout24Provider(downloader, log);

            var search = appConfig.SearchConfig.FirstOrDefault(p => p.DescriptionShort == "ImmoScout24 Treptow-Köpenick");
            var index = await provider.LoadIndexAsync(search);
        }
    }
}
