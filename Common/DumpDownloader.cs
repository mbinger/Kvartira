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

        public static async Task<Response> ReadDumpAsync(string url, string dump, bool deflate)
        {
            var regexTrimStart = new Regex("^<!--");
            var regexTrimEnd = new Regex("-->$");

            Response result;

            using (var textReader = new StringReader(dump))
            {
                var urlText = await textReader.ReadLineAsync();
                var httpStatusCodeText = await textReader.ReadLineAsync();
                var exceptionText = await textReader.ReadLineAsync();
                var content = await textReader.ReadToEndAsync();

                if (regexTrimStart.IsMatch(urlText) && regexTrimEnd.IsMatch(urlText) &&
                    regexTrimStart.IsMatch(httpStatusCodeText) && regexTrimEnd.IsMatch(httpStatusCodeText) &&
                    regexTrimStart.IsMatch(exceptionText) && regexTrimEnd.IsMatch(exceptionText))
                {
                    urlText = regexTrimEnd.Replace(regexTrimStart.Replace(urlText, ""), "");
                    httpStatusCodeText = regexTrimEnd.Replace(regexTrimStart.Replace(httpStatusCodeText, ""), "");
                    exceptionText = regexTrimEnd.Replace(regexTrimStart.Replace(exceptionText, ""), "");

                    result = new Response
                    {
                        Url = urlText,
                        HttpStatusCode = int.Parse(httpStatusCodeText),
                        Exception = string.IsNullOrEmpty(exceptionText) ? null : exceptionText,
                        Content = content
                    };
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(urlText);
                    sb.AppendLine(httpStatusCodeText);
                    sb.AppendLine(exceptionText);
                    sb.AppendLine(content);

                    result = new Response
                    {
                        Url = url,
                        HttpStatusCode = 0,
                        Exception = null,
                        Content = sb.ToString()
                    };
                }

                if (deflate)
                {
                    result.Content = Scanner.DeflateHtml(result.Content);
                }

                return result;
            }
        }

        public async Task<Response> GetAsync(string url, bool fromcache, string description, bool deflate)
        {
            if (index >= dumpFiles.Length)
            {
                throw new Exception("Dumps cache empty");
            }

            var fileName = dumpFiles[index];
            index++;
            var path = Path.Combine(appConfig.DumpFolder, fileName);

            using (var textReader = new StreamReader(path))
            {
                var dump = await textReader.ReadToEndAsync();
                return await ReadDumpAsync(url, dump, deflate);
            }
        }

        public static void WriteDump(AppConfig appConfig, ILog log, Response response, string description)
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

                File.WriteAllText(path, content.ToString());
            }
            catch (Exception ex)
            {
                log.Write($"Error dumping content\n {ex}");
            }
        }
    }
}
