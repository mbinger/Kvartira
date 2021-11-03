using Common;
using Data;
using Newtonsoft.Json;
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

            /*
            using (var context = new WohnungDb())
            {
                context.Database.EnsureCreated();
            }
            */
            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
        }

        public async Task Work()
        {
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("Config.json"));
            WohnungDb.AppConfig = appConfig;

            var log = new Log(appConfig);
            var downloader = new Downloader(log, appConfig);

            var response = await downloader.GetAsync("https://www.delftstack.com/howto/csharp/write-to-debug-window-in-csharp/", "test");
        }
    }
}
