using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.Degewo
{
    public class DegewoProvider: ProviderBase
    {
        public DegewoProvider(IDownloader downloader, ILog log, ILog rulog) : base(downloader, log, rulog)
        {
        }

        public override bool NeedZombieBrowser { get; } = false;
        public override string Name { get; } = "DEGEWO";
        protected override string DetailsUrl { get; } = "https://immosuche.degewo.de/de/properties/{0}";

        protected override Parser NichtsGefundenParser { get; } = new Parser((content) => content.Contains("Ihre Suche ist zu spezifisch und liefert keine Ergebnisse") ? True : False);

        protected override Regex GetIdsRegex { get; } = new Regex("href=\"/de/properties/(?<value>[0-9a-zA-Z\\-]+)\"");

        protected override Regex IndexNextPageUrlRegex { get; } = null;
        protected override string GetNextPageUrl(string content, string currentUrl, int currentPage, List<string> ids)
        {
            if (!ids.Any())
            {
                return null;
            }

            if (ids.Count < 10)
            {
                return null;
            }

            var nextUrl = currentUrl.Replace($"page={currentPage}", $"page={currentPage + 1}");
            return nextUrl;
        }

        protected override Parser DetailsHeaderParser { get; } = new Parser(new Regex("<h1 class='article__title'>(?<value>[^<]+)</h1>"));

        protected override Parser DetailsGrundMieteParser { get; } = new Parser(new Regex("Nettokaltmiete\\W*(?<value>[\\d,\\.]+)"));

        protected override Parser DetailsGesamtMieteParser { get; } = new Parser(new Regex("<div class='expose__price-tag'>(?<value>[\\d,\\.]+)"));

        protected override Parser DetailsKautionParser { get; } = null;

        protected override Parser DetailsAnschriftParser { get; } = new Parser(new Regex("<p class='gallery__lead'>(?<value>[\\w\\d<>\\s-\\.]+)</p>"), (content) => content.Replace("<br>", " "));

        protected override Parser DetailsBezirkParser { get; } = null;

        protected override Parser[] DetailsBeschreibungParser { get; } = new[] { new Parser(new Regex("Ausstattung</cite><p>(?<value>[^<]+)</p>")) };

        protected override Parser DetailsEtageParser { get; } = new Parser(new Regex("Geschoss / Anzahl</td><td class='teaser-tileset__table-item'>(?<value>\\d+)\\W*/\\W*\\d+</td></tr>"));

        protected override Parser DetailsEtagenParser { get; } = new Parser(new Regex("Geschoss / Anzahl</td><td class='teaser-tileset__table-item'>\\d+\\W*/\\W*(?<value>\\d+)</td></tr>"));

        protected override Parser DetailsWbsParser { get; } = new Parser((content) => content.Contains("WBS benötigt</td><td class='teaser-tileset__table-item'>Ja</td></tr>") ? True : False);

        protected override Parser DetailsZimmerParser { get; } = new Parser(new Regex("Zimmer</td><td class='teaser-tileset__table-item'>(?<value>\\d+)"));

        protected override Parser DetailsFlaecheParser { get; } = new Parser(new Regex("Wohnfläche</td><td class='teaser-tileset__table-item'>(?<value>[\\d,]+)"));

        protected override Parser DetailsFreiAbParser { get; } = new Parser(new Regex("Verfügbar ab</td><td class='teaser-tileset__table-item'>(?<value>[^<]+)"), (value) =>
        {
            if (value == "sofort")
            {
                return DateTime.Today.ToString("dd.MM.yyyy");
            }
            return value;
        });

        protected override Parser DetailsBalkonParser { get; } = null;

        protected override Parser DetailsKellerParser { get; } = null;

        protected override async Task<WohnungCard> ParseDetailsAsync(string content, string wohnungId, string description)
        {
            var card = await base.ParseDetailsAsync(content, wohnungId, description);

            if (card.Wbs == null)
            {
                card.Wbs = card.Header.ToUpper().Contains("WBS");
            }

            var kautionParser = new Parser(new Regex("Kaution:\\W*(?<value>[^<]+)"));
            var kaution = await Scanner.ParseSafeAsync(kautionParser, "kaution", content, description, log);

            card.Balkon = card.Beschreibung.ToLower().Contains("balkon");

            if (!string.IsNullOrEmpty(kaution))
            {
                var comma = !string.IsNullOrEmpty(card.Beschreibung) ? ", " : "";
                card.Beschreibung = "Kaution: " + kaution + comma + card.Beschreibung;
            }

            card.Complete = !string.IsNullOrEmpty(card.Header) &&
              !string.IsNullOrEmpty(card.Anschrift) &&
              card.Balkon != null &&
              !string.IsNullOrEmpty(card.Beschreibung) &&
              //!string.IsNullOrEmpty(card.Bezirk) bezirk null
              card.Etage != null &&
              card.Etagen != null &&
              card.Flaeche != null &&
              card.FreiAb != null &&
              card.Wbs != null &&
              //card.Keller != null && keller null
              card.Zimmer != null &&
              card.MieteKalt != null &&
              card.MieteWarm != null;
              //card.Kaution != null; Kaution null

            return card;
        }
    }
}
