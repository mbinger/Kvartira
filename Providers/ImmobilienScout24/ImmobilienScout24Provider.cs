using Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.ImmobilienScout24
{
    public class ImmobilienScout24Provider : ProviderBase
    {
        public ImmobilienScout24Provider(IDownloader downloader, ILog log) : base(downloader, log)
        {
        }

        public override string Name { get; } = "IMMO SCOUT 24";

        protected override string DetailsUrl { get; } = "https://www.immobilienscout24.de/expose/{0}#/";

        protected override Parser NichtsGefundenParser { get; } = new Parser((content) => content.Contains("Leider haben wir aktuell keine Treffer für Ihre Suchkriterien!") ? True : False);

        private const string Roboter = "Ich bin kein Roboter";

        protected override async Task<Response> Download(string url, bool fromcache, string description)
        {
            var usecache = fromcache;
            for (var i = 0; i <= 4; i++)
            {
                var content = await base.Download(url, usecache, description);
                if (content.HttpStatusCode == 200 && content.Content.Contains(Roboter))
                {
                    usecache = i % 2 == 0;
                    await downloader.Delay();
                    continue;
                }
                else
                {
                    return content;
                }
            }

            return new Response
            {
                Exception = "CAPTCHA detected!"
            };
        }

        protected override Regex GetIdsRegex { get; } = new Regex("data-id=\"(?<value>\\d+)\"");

        protected override Regex IndexNextPageUrlRegex { get; } = null;
        protected override string GetNextPageUrl(string content, string currentUrl, int currentPage, List<string> ids)
        {
            if (content.Contains("aria-disabled=\"true\" aria-label=\"Next page\">"))
            {
                //last page
                return null;
            }

            if (!currentUrl.Contains("&pagenumber="))
            {
                var nextUrl1 = currentUrl + $"&pagenumber={currentPage + 1}";
                return nextUrl1;
            }

            var nextUrl2 = currentUrl.Replace($"pagenumber={currentPage}", $"pagenumber={currentPage + 1}");
            return nextUrl2;
        }

        protected override Parser DetailsHeaderParser { get; } = new Parser(new Regex("<h1\\s.+id=\"expose-title\"[^>]+>(?<value>[^<]+)"));

        protected override Parser DetailsGrundMieteParser { get; } = new Parser(new Regex("<div class=\"is24qa-kaltmiete-main[^\"]*\">(?<value>[^<]+)"), (val) => val?.Replace("EUR", "")?.Replace("€", "")?.Trim());

        protected override Parser DetailsGesamtMieteParser { get; } = new Parser(new Regex("<div class=\"is24qa-warmmiete-main[^\"]*\">(?<value>[^<]+)"), (val) => val?.Replace("EUR", "")?.Replace("€", "")?.Trim());

        protected override Parser DetailsKautionParser { get; } = new Parser(new Regex("<div class=\"is24qa-kaution-o-genossenschaftsanteile[^\"]*\">(?<value>[^<]+)"), (val) => val?.Replace("EUR", "")?.Replace("€", "")?.Trim());

        protected override Parser DetailsAnschriftParser { get; } = new Parser(new Regex("<div class=\"address-block\">\\s*<div\\s[^>]+>\\s*<span\\s[^>]+>(?<value1>[^<]+)</span>\\s*<span\\s[^>]+>(?<value2>[^<]+)"), (val) => val?.Trim());

        protected override Parser DetailsBezirkParser { get; } = new Parser(new Regex("<div class=\"address-block\">\\s*<div\\s[^>]+>\\s*<span\\s[^>]+>[^<]+</span>\\s*<span\\s[^>]+>\\d+(?<value>[^<]+)"), (val) => val?.Replace("Berlin", "")?.Replace(",", "")?.Trim());

        protected override Parser[] DetailsBeschreibungParser { get; } = new[]
        {
            new Parser(new Regex("<pre class=\"is24qa-objektbeschreibung[^>]*>(?<value>[^<]+)</pre>")),
            new Parser(new Regex("<pre class=\"is24qa-ausstattung[^>]*>(?<value>[^<]+)</pre>"))
        };

        protected override Parser DetailsEtageParser { get; } = new Parser(new Regex("<dd class=\"is24qa-etage[^\"]*\">\\s*(?<value>\\d+)"));

        protected override Parser DetailsEtagenParser { get; } = new Parser(new Regex("<dd class=\"is24qa-etage[^\"]*\">\\s*\\d*\\s*von\\s*(?<value>\\d+)"));

        protected override Parser DetailsWbsParser { get; } = null;

        protected override Parser DetailsZimmerParser { get; } = new Parser(new Regex("<dd class=\"is24qa-zimmer\\s[^>]+>(?<value>[^<]+)"));

        protected override Parser DetailsFlaecheParser { get; } = new Parser(new Regex("<dd class=\"is24qa-wohnflaeche-ca[^\"]*\">(?<value>[^<]+)"));

        protected override Parser DetailsFreiAbParser { get; } = new Parser(new Regex("<dd class=\"is24qa-bezugsfrei-ab\\s[^>]+>(?<value>[^<]+)"), (value) =>
        {
            value = value?.Trim();
            if (value == "sofort")
            {
                return DateTime.Today.ToString("dd.MM.yyyy");
            }
            return value;
        });

        protected override Parser DetailsBalkonParser { get; } = new Parser((val) => val.Contains("<span class=\"is24qa-balkon-terrasse-label ") ? True : False);

        protected override Parser DetailsKellerParser { get; } = new Parser((val) => val.Contains("<span class=\"is24qa-keller-label ") ? True : False);

        protected override async Task<WohnungCard> ParseDetailsAsync(string content, string wohnungId, string description)
        {
            var card = await base.ParseDetailsAsync(content, wohnungId, description);

            if (card.Wbs == null)
            {
                card.Wbs = card.Header.ToUpper().Contains("WBS") || card.Beschreibung.ToUpper().Contains("WBS");
            }

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
    }
}
