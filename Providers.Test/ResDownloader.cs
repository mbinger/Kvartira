using Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
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

        public ResDownloader(byte[][] res, string saveDeflatedToPath = null)
        {
            this.res = new string[res.Length];
            for (var i = 0; i < res.Length; i++)
            {
                byte[] content;
                if (res[i].Length >= 2 && 
                    res[i][0] == 80 && //P
                    res[i][1] == 75)   //K
                {
                    //decompress
                    content = Decompress(res[i]);
                }
                else
                {
                    content = res[i];
                }

                var contentStr = Encoding.UTF8.GetString(content);
                this.res[i] = contentStr;
            }
            this.saveDeflatedToPath = saveDeflatedToPath;
            index = 0;
        }

        static byte[] Decompress(byte[] data)
        {
            using (var resultStream = new MemoryStream())
            {
                using (var compressedStream = new MemoryStream(data))
                {
                    using (var archive = new ZipArchive(compressedStream, ZipArchiveMode.Read))
                    {
                        if (archive.Entries.Count == 0)
                        {
                            throw new Exception("ZIP archive is empty");
                        }
                        if (archive.Entries.Count > 1)
                        {
                            throw new Exception("Only 1 file per ZIP archive supported");
                        }
                        var entry = archive.Entries[0];
                        using (var entryStream = entry.Open())
                        {
                            entryStream.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    }
                }
            }
        }

        public Task Delay()
        {
            return Task.CompletedTask;
        }

        public async Task<Response> GetAsync(string url, bool fromcache, string description, bool deflate)
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
