using Common;
using Data;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("Config.json"));
            WohnungDb.AppConfig = appConfig;

            using (var context = new WohnungDb())
            {
                context.Database.EnsureCreated();
            }

            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
        }
    }
}
