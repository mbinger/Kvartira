using Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Providers.Test
{
    public class ResDownloader : IDownloader
    {
        private readonly string[] res;
        private readonly string saveDeflatedToPath;
        private int index;

        public ResDownloader(string [] res, string saveDeflatedToPath = null)
        {
            this.res = res;
            this.saveDeflatedToPath = saveDeflatedToPath;
            index = 0;
        }

        public Task Delay()
        {
            return Task.CompletedTask;
        }

        public async Task<Response> GetAsync(string url, string description, bool deflate)
        {
            if (index >= res.Length)
            {
                throw new Exception("Dumps cache empty");
            }

            var dump = res[index];
            var response = await DumpDownloader.ReadDumpAsync(url, dump, deflate);

            if (!string.IsNullOrEmpty(saveDeflatedToPath))
            {
                var path = Path.Combine(saveDeflatedToPath, $"deflated_{index}.htm");
                File.WriteAllText(path, response.Content);
            }

            index++;

            return response;
        }
    }
}
