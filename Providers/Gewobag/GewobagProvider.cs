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
        public const string DetailsUrl = "https://www.gewobag.de/fuer-mieter-und-mietinteressenten/mietangebote/{0}/";

        private readonly IDownloader downloader;
        private readonly Log log;

        public GewobagProvider(IDownloader downloader, Log log)
        {
            this.downloader = downloader;
            this.log = log;
        }

        public Task<LoadIdsResult> LoadIndexAsync(Search search)
        {
            return LoadIndexPrivateAsync(search.SearchUrl, search.DescriptionShort, 1);
        }

        private async Task<LoadIdsResult> LoadIndexPrivateAsync(string url, string description, int page)
        {
            try
            {
                await downloader.Delay();

                var content = await downloader.GetAsync(url, Name + "_" + description + $" page {page}", true);

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

                await downloader.Delay();

                var nextPageResult = await LoadIndexPrivateAsync(nextPageUrl, description, page + 1);

                result.WohnungIds.AddRange(nextPageResult.WohnungIds);
                result.PagesCount += nextPageResult.PagesCount;

                return result;
            }
            catch (Exception ex)
            {
                await log.LogAsync($"ERROR {nameof(GewobagProvider)}.{nameof(LoadIndexAsync)}\n" + ex.ToString());
            }
            return null;
        }

        public string GetOpenDetailsUrl(string wohnungId)
        {
            return string.Format(DetailsUrl, wohnungId);
        }

        public async Task<WohnungCard> LoadDetailsAsync(string wohnungId)
        {
            try
            {
                await downloader.Delay();
                var url = GetOpenDetailsUrl(wohnungId);
                var response = await downloader.GetAsync(url, Name + "_details_" + wohnungId, true);
                if (!string.IsNullOrEmpty(response.Exception))
                {
                    return null;
                }

                var headerRegex = new Regex("<h1 class=\"entry-title\">(?<value>[^>]+)</h1>");
                var grundMieteRegex = new Regex("Grundmiete</div><div[^>]+>(?<value>[0-9\\,]+)");
                var gesamtMieteRegex = new Regex("Gesamtmiete</div><div[^>]+>(?<value>[0-9\\,]+)");
                var kautionRegex = new Regex("Kaution</div><div[^>]+>(?<value>[0-9\\,]+)");
                var anschriftRegex = new Regex("Anschrift</div><div[^>]+>(?<value>[^>]+)</div>");
                var bezirkRegex = new Regex("Bezirk/Ortsteil</div><div[^>]+>(?<value>[^>]+)</div>");
                var beschreibungRegex = new Regex("Beschreibung</div><div[^>]+>(?<value>[^>]+)</div>");
                var etageregex = new Regex("Etage</div><div[^>]+>(?<value>\\d+)</div>");
                var zimmerRegex = new Regex("Anzahl Zimmer</div><div[^>]+>(?<value>\\d+)</div>");
                var flaecheRegex = new Regex("Fläche in m²</div><div[^>]+>(?<value>[\\d\\,]+)");
                var freiAbRegex = new Regex("Frei ab</div><div[^>]+>(?<value>[^>]+)</div>");

                var description = $"{Name} - {wohnungId}";

                var header = await Scanner.ParseSafeAsync(headerRegex, "header", response.Content, description, log);
                var card = new WohnungCard
                {
                    Header = header,
                    Anschrift = await Scanner.ParseSafeAsync(anschriftRegex, "anschrift", response.Content, description, log),
                    Balkon = response.Content.Contains("<li>Balkon/Terasse</li>"),
                    Beschreibung = await Scanner.ParseSafeAsync(beschreibungRegex, "beschreibung", response.Content, description, log),
                    Bezirk = await Scanner.ParseSafeAsync(bezirkRegex, "bezirk", response.Content, description, log),
                    Etage = await Scanner.ParseSafeIntAsync(etageregex, "bezirk", response.Content, description, log),
                    Flaeche = await Scanner.ParseSafeDecimalAsync(flaecheRegex, "flaeche", response.Content, description, log),
                    FreiAb = await Scanner.ParseSafeDateAsync(freiAbRegex, "freiab", response.Content, description, log),
                    Wbs = header.Contains("WBS", StringComparison.InvariantCultureIgnoreCase),
                    Keller = response.Content.Contains("<li>Keller</li>"),
                    Zimmer = await Scanner.ParseSafeIntAsync(zimmerRegex, "zimmer", response.Content, description, log),
                    MieteKalt = await Scanner.ParseSafeDecimalAsync(grundMieteRegex, "mieteKalt", response.Content, description, log),
                    MieteWarm = await Scanner.ParseSafeDecimalAsync(gesamtMieteRegex, "mieteWarm", response.Content, description, log),
                    Kaution = await Scanner.ParseSafeDecimalAsync(kautionRegex, "kaution", response.Content, description, log)
                };

                return card;
            }
            catch (Exception ex)
            {
                await log.LogAsync($"ERROR {nameof(GewobagProvider)}.{nameof(LoadDetailsAsync)}\n" + ex.ToString());
            }

            return null;
        }
    }
}
