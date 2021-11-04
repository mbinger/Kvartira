using Common;
using Data;
using Newtonsoft.Json;
using Providers;
using Providers.Gewobag;
using System;
using System.IO;
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

            //var downloader = new HttpDownloader(log, appConfig);
            
            var downloader = new DumpDownloader(appConfig, new[]
            {
                "2021_11_04_11_19_30_GEWOBAG_details_0100-02571-0103-0124.htm",
            });

            var gewobag = new GewobagProvider(downloader, log);
            var providers = new IProvider[]
            {
                gewobag
            };

            //var director = new Director(appConfig, providers, log);

            //var count = await director.LoadAsync();

            //Console.WriteLine($"{count} entries loaded");

            /*
            var index = await gewobag.LoadIndexAsync(new Search
            {
                DescriptionLong = "",
                DescriptionShort = "",
                Importance = 0,
                ProviderName = "GEWOBAG",
                SearchUrl = "",
                Active = true
            });*/
            var details = await gewobag.LoadDetailsAsync("0100-02571-0103-0124");
        }
    }
}
