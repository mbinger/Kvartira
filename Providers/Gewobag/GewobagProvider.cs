using Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.Gewobag
{
    public class GewobagProvider : IProvider
    {
        public string Name => "GEWOBAG";

        private readonly IDownloader downloader;
        private readonly Log log;

        public GewobagProvider(IDownloader downloader, Log log)
        {
            this.downloader = downloader;
            this.log = log;
        }

        public Task<LoadIdsResult> LoadIdsAsync(Search search)
        {
            return LoadIdsPrivateAsync(search.SearchUrl, search.Description, 1);
        }

        private async Task<LoadIdsResult> LoadIdsPrivateAsync(string url, string description, int page)
        {
            var content = await downloader.GetAsync(url, Name + "_" + description + $" page {page}");

            await log.LogAsync($"{Name} {description} GET page {page} {url} HTTP {content.HttpStatusCode} {content.Exception}");

            if (!string.IsNullOrEmpty(content.Exception) || string.IsNullOrEmpty(content.Content))
            {
                return new LoadIdsResult
                {
                    PagesCount = 0
                };
            }

            //get IDS
            //<a href="https://www.gewobag.de/fuer-mieter-und-mietinteressenten/mietangebote/0100-01711-0204-0119/" class="read-more-link">Mietangebot ansehen</ a >
            var getIdsRegex = new Regex("https://www.gewobag.de/fuer-mieter-und-mietinteressenten/mietangebote/(?<id>[0-9a-zA-Z\\-]+)/");

            var matches = getIdsRegex.Matches(content.Content);

            var ids = matches.Cast<Match>().Select(p => p.Groups["id"].Value).Distinct().Where(p => p != "page").ToList();

            var result = new LoadIdsResult
            {
                WohnungIds = ids,
                PagesCount = 1
            };

            //next page exists?
            var nextPageRegex = new Regex("<a class=\"next page-numbers\" href=\"(?<nextPageUrl>.+)\">Weiter &raquo;</a>");
            var nextPageMatch = nextPageRegex.Match(content.Content);

            if (!nextPageMatch.Success)
            {
                return result;
            }

            //load next page
            var nextPageUrl = nextPageMatch.Groups["nextPageUrl"].Value;

            var nextPageResult = await LoadIdsPrivateAsync(nextPageUrl, description, page + 1);

            result.WohnungIds.AddRange(nextPageResult.WohnungIds);
            result.PagesCount += nextPageResult.PagesCount;

            return result;
        }

        public Task<WohnungCard> LoadDetailsAsync(string wohnungId)
        {
            throw new NotImplementedException();
        }
    }
}
