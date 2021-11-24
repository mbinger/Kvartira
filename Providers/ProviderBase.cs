using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers
{
    public abstract class ProviderBase : IProvider
    {
        protected const string True = "True";
        protected const string False = "False";

        public abstract string Name { get; }
        protected abstract string DetailsUrl { get; }

        protected abstract Parser NichtsGefundenParser { get; }
        protected abstract Regex GetIdsRegex { get; }
        protected abstract Regex IndexNextPageUrlRegex { get; }

        protected abstract Parser DetailsHeaderParser { get; }
        protected abstract Parser DetailsGrundMieteParser { get; }
        protected abstract Parser DetailsGesamtMieteParser { get; }
        protected abstract Parser DetailsKautionParser { get; }
        protected abstract Parser DetailsAnschriftParser { get; }
        protected abstract Parser DetailsBezirkParser { get; }
        protected abstract Parser DetailsBeschreibungParser { get; }
        protected abstract Parser DetailsEtageParser { get; }
        protected abstract Parser DetailsEtagenParser { get; }
        protected abstract Parser DetailsWbsParser { get; }
        protected abstract Parser DetailsZimmerParser { get; }
        protected abstract Parser DetailsFlaecheParser { get; }
        protected abstract Parser DetailsFreiAbParser { get; }
        protected abstract Parser DetailsBalkonParser { get; }
        protected abstract Parser DetailsKellerParser { get; }

        protected readonly IDownloader downloader;
        protected readonly ILog log;

        public ProviderBase(IDownloader downloader, ILog log)
        {
            this.downloader = downloader;
            this.log = log;
        }

        public virtual Task<LoadIdsResult> LoadIndexAsync(Search search)
        {
            return LoadIndexPrivateAsync(search.SearchUrl, search.DescriptionShort, 1);
        }

        protected virtual List<string> ParseIds(string indexContent)
        {
            var getIdsRegex = GetIdsRegex;
            var matches = getIdsRegex.Matches(indexContent);
            var ids = matches.Cast<Match>().Select(p => p.Groups["value"].Value).Distinct().ToList();
            return ids;
        }

        protected virtual async Task<WohnungCard> ParseDetailsAsync(string content, string wohnungId, string description)
        {
            var header = await Scanner.ParseSafeAsync(DetailsHeaderParser, "header", content, description, log);
            var anschrift = await Scanner.ParseSafeAsync(DetailsAnschriftParser, "anschrift", content, description, log);
            var balkon = await Scanner.ParseSafeBoolAsync(DetailsBalkonParser, "balkon", content, description, log);
            var beschreibung = await Scanner.ParseSafeAsync(DetailsBeschreibungParser, "beschreibung", content, description, log);
            var bezirk = await Scanner.ParseSafeAsync(DetailsBezirkParser, "bezirk", content, description, log);
            var etage = await Scanner.ParseSafeIntAsync(DetailsEtageParser, "etage", content, description, log);
            var etagen = await Scanner.ParseSafeIntAsync(DetailsEtagenParser, "etagen", content, description, log);
            var flaeche = await Scanner.ParseSafeDecimalAsync(DetailsFlaecheParser, "flaeche", content, description, log);
            var freiAb = await Scanner.ParseSafeDateAsync(DetailsFreiAbParser, "freiab", content, description, log);
            var wbs = await Scanner.ParseSafeBoolAsync(DetailsWbsParser, "wbs", content, description, log);
            var keller = await Scanner.ParseSafeBoolAsync(DetailsKellerParser, "keller", content, description, log);
            var zimmer = await Scanner.ParseSafeIntAsync(DetailsZimmerParser, "zimmer", content, description, log);
            var mieteKalt = await Scanner.ParseSafeDecimalAsync(DetailsGrundMieteParser, "mieteKalt", content, description, log);
            var mieteWarm = await Scanner.ParseSafeDecimalAsync(DetailsGesamtMieteParser, "mieteWarm", content, description, log);
            var kaution = await Scanner.ParseSafeDecimalAsync(DetailsKautionParser, "kaution", content, description, log);

            var card = new WohnungCard
            {
                Header = header,
                Anschrift = anschrift,
                Balkon = balkon,
                Beschreibung = beschreibung,
                Bezirk = bezirk,
                Etage = etage,
                Etagen = etagen,
                Flaeche = flaeche,
                FreiAb = freiAb,
                Wbs = wbs,
                Keller = keller,
                Zimmer = zimmer,
                MieteKalt = mieteKalt,
                MieteWarm = mieteWarm,
                Kaution = kaution
            };

            card.Complete = !string.IsNullOrEmpty(card.Header) &&
                !string.IsNullOrEmpty(card.Anschrift) &&
                card.Balkon != null &&
                !string.IsNullOrEmpty(card.Beschreibung) &&
                !string.IsNullOrEmpty(card.Bezirk) &&
                card.Etage != null &&
                card.Etagen != null &&
                card.Flaeche != null &&
                card.FreiAb != null &&
                card.Wbs != null &&
                card.Keller != null &&
                card.Zimmer != null &&
                card.MieteKalt != null &&
                card.MieteWarm != null &&
                card.Kaution != null;

            return card;
        }

        protected virtual string GetNextPageUrl(string content, string currentUrl, int currentPage, List<string> ids)
        {
            //next page exists?
            var nextPageRegex = IndexNextPageUrlRegex;
            var nextPageMatch = nextPageRegex.Match(content);

            if (!nextPageMatch.Success)
            {
                return null;
            }

            //load next page
            var nextPageUrl = nextPageMatch.Groups["value"].Value;
            return nextPageUrl;
        }

        private async Task<LoadIdsResult> LoadIndexPrivateAsync(string url, string description, int page)
        {
            try
            {
                await downloader.Delay();

                var content = await downloader.GetAsync(url, Name + "_" + description + $" page {page}", true);

                log.Write($"{Name} {description} GET page {page} {url} HTTP {content.HttpStatusCode} {content.Exception}");

                if (!string.IsNullOrEmpty(content.Exception) || string.IsNullOrEmpty(content.Content))
                {
                    return new LoadIdsResult
                    {
                        PagesCount = 0
                    };
                }

                var nichtsGefunden = await Scanner.ParseSafeBoolAsync(NichtsGefundenParser, "index-nichts-gefunden", content.Content, description, log);
                if (nichtsGefunden == true)
                {
                    return new LoadIdsResult
                    {
                        PagesCount = 0
                    };
                }

                //get IDS
                var ids = ParseIds(content.Content);

                var result = new LoadIdsResult
                {
                    WohnungIds = ids,
                    PagesCount = 1
                };


                var nextPageUrl = GetNextPageUrl(content.Content, url, page, ids);
                if (string.IsNullOrEmpty(nextPageUrl))
                {
                    return result;
                }

                await downloader.Delay();

                var nextPageResult = await LoadIndexPrivateAsync(nextPageUrl, description, page + 1);

                result.WohnungIds.AddRange(nextPageResult.WohnungIds);
                result.PagesCount += nextPageResult.PagesCount;

                return result;
            }
            catch (Exception ex)
            {
                log.Write($"ERROR {GetType().Name}.{nameof(LoadIndexAsync)}\n" + ex.ToString());
            }
            return null;
        }

        public string GetOpenDetailsUrl(string wohnungId)
        {
            return string.Format(DetailsUrl, wohnungId);
        }


        public async Task<WohnungCard> LoadDetailsAsync(string wohnungId, bool immediately = false)
        {
            try
            {
                if (!immediately)
                {
                    await downloader.Delay();
                }
                var url = GetOpenDetailsUrl(wohnungId);
                var response = await downloader.GetAsync(url, Name + "_details_" + wohnungId, true);
                if (!string.IsNullOrEmpty(response.Exception))
                {
                    return null;
                }

                var description = $"{Name} - {wohnungId}";

                var card = await ParseDetailsAsync(response.Content, wohnungId, description);
                return card;
            }
            catch (Exception ex)
            {
                log.Write($"ERROR {GetType().Name}.{nameof(LoadDetailsAsync)}\n" + ex.ToString());
            }

            return null;
        }
    }
}
