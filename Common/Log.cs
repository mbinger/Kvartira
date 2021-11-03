using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Common
{
    public class Log
    {
        private readonly AppConfig appConfig;

        public Log(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        public async Task LogAsync(string str)
        {
            try
            {
                var logItem = $"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss")}] {str}";
                Console.WriteLine(logItem);
                Debug.WriteLine(logItem);
                await File.AppendAllLinesAsync(appConfig.LogFile, new[] { logItem });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debug.WriteLine(ex);
            }
        }
    }
}
