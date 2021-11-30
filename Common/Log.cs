using System;
using System.Diagnostics;
using System.IO;

namespace Common
{
    public class Log: ILog
    {
        private readonly AppConfig appConfig;

        public Log(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        public void Write(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(appConfig.LogFile))
                {
                    var logItem = $"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}] {str}";
                    Console.WriteLine(logItem);
                    Debug.WriteLine(logItem);
                    File.AppendAllLines(appConfig.LogFile, new[] { logItem });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debug.WriteLine(ex);
            }
        }
    }
}
