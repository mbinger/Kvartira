using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class DumpDownloader : IDownloader
    {
        private readonly AppConfig appConfig;
        private readonly string[] dumpFiles;
        private int index;

        public DumpDownloader(AppConfig appConfig, string[] dumpFiles)
        {
            this.appConfig = appConfig;
            this.dumpFiles = dumpFiles;
            index = 0;
        }

        public Task Delay()
        {
            return Task.CompletedTask;
        }

        public async Task<Response> GetAsync(string url, string description)
        {
            if (index >= dumpFiles.Length)
            {
                throw new Exception("Dumps cache empty");
            }

            var fileName = dumpFiles[index];
            var path = Path.Combine(appConfig.DumpFolder, fileName);

            var regexTrimStart = new Regex("^<!--");
            var regexTrimEnd = new Regex("-->$");

            using (var textReader = new StreamReader(path))
            {
                var urlText = await textReader.ReadLineAsync();
                var httpStatusCodeText = await textReader.ReadLineAsync();
                var exceptionText = await textReader.ReadLineAsync();
                var content = await textReader.ReadToEndAsync();

                urlText = regexTrimEnd.Replace(regexTrimStart.Replace(urlText, ""), "");
                httpStatusCodeText = regexTrimEnd.Replace(regexTrimStart.Replace(httpStatusCodeText, ""), "");
                exceptionText = regexTrimEnd.Replace(regexTrimStart.Replace(exceptionText, ""), "");

                index++;

                var result = new Response
                {
                    Url = urlText,
                    HttpStatusCode = int.Parse(httpStatusCodeText),
                    Exception = string.IsNullOrEmpty(exceptionText) ? null : exceptionText,
                    Content = content
                };

                return result;
            }
        }

        public static async Task WriteDumpAsync(AppConfig appConfig, Log log, Response response, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(appConfig.DumpFolder))
                {
                    return;
                }

                var fileName = $"{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}_{description}.htm";
                var path = Path.Combine(appConfig.DumpFolder, fileName);

                if (!Directory.Exists(appConfig.DumpFolder))
                {
                    Directory.CreateDirectory(appConfig.DumpFolder);
                }

                var content = new StringBuilder();
                content.AppendLine($"<!--{response.Url}-->");
                content.AppendLine($"<!--{response.HttpStatusCode.ToString()}-->");
                content.AppendLine($"<!--{response.Exception?.Replace("\n", "")?.Replace("\r", "")}-->");
                content.AppendLine(response.Content);

                await File.WriteAllTextAsync(path, content.ToString());
            }
            catch (Exception ex)
            {
                await log.LogAsync($"Error dumping content\n {ex}");
            }
        }
    }
}
