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
        static void Main(string[] args)
        {
            var program = new Program();

            Task.Run(async () => await program.Work()).GetAwaiter().GetResult();

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
                "2021_11_03_12_38_58_GEWOBAG_Test Gewobag load all page 1.htm",
                "2021_11_03_12_39_04_GEWOBAG_Test Gewobag load all page 2.htm"
            });
            
            var providers = new IProvider[]
            {
                new GewobagProvider(downloader, log)
            };

            var director = new Director(appConfig, providers, log);

            var count = await director.LoadAsync();

            Console.WriteLine($"{count} entries loaded");
        }
    }
}
