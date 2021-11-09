using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.Gewobag
{
    public class GewobagProvider : ProviderBase
    {
        public GewobagProvider(IDownloader downloader, Log log) : base(downloader, log)
        {
        }
        
        public override string Name { get; } = "GEWOBAG";
        protected override string DetailsUrl { get; } = "https://www.gewobag.de/fuer-mieter-und-mietinteressenten/mietangebote/{0}/";
        
        protected override Regex GetIdsRegex => new Regex("https://www.gewobag.de/fuer-mieter-und-mietinteressenten/mietangebote/(?<value>[0-9a-zA-Z\\-]+)/");
        protected override Parser NichtsGefundenParser { get; } = new Parser((content) => content.Contains("Es konnten keine passenden Angebote gefunden werden") ? True : False);
        protected override Regex IndexNextPageUrlRegex { get; } = new Regex("<a class=\"next page-numbers\" href=\"(?<value>.+)\">Weiter &raquo;</a>");

        protected override Parser DetailsHeaderParser { get; } = new Parser(new Regex("<h1 class=\"entry-title\">(?<value>[^>]+)</h1>"));
        protected override Parser DetailsGrundMieteParser { get; } = new Parser(new Regex("Grundmiete</div><div[^>]+>(?<value>[0-9\\,]+)"));
        protected override Parser DetailsGesamtMieteParser { get; } = new Parser(new Regex("Gesamtmiete</div><div[^>]+>(?<value>[0-9\\,]+)"));
        protected override Parser DetailsKautionParser { get; } = new Parser(new Regex("Kaution</div><div[^>]+>(?<value>[0-9\\,]+)"));
        protected override Parser DetailsAnschriftParser { get; } = new Parser(new Regex("Anschrift</div><div[^>]+>(?<value>[^>]+)</div>"));
        protected override Parser DetailsBezirkParser { get; } = new Parser(new Regex("Bezirk/Ortsteil</div><div[^>]+>(?<value>[^>]+)</div>"));
        protected override Parser DetailsBeschreibungParser { get; } = new Parser(new Regex("Beschreibung</div><div[^>]+>(?<value>[^>]+)</div>"));
        protected override Parser DetailsEtageParser { get; } = new Parser(new Regex("Etage</div><div[^>]+>(?<value>\\d+)</div>"));
        protected override Parser DetailsEtagenParser { get; } = null;
        protected override Parser DetailsZimmerParser { get; } = new Parser(new Regex("Anzahl Zimmer</div><div[^>]+>(?<value>\\d+)</div>"));
        protected override Parser DetailsFlaecheParser { get; } = new Parser(new Regex("Fläche in m²</div><div[^>]+>(?<value>[\\d\\,]+)"));
        protected override Parser DetailsFreiAbParser { get; } = new Parser(new Regex("Frei ab</div><div[^>]+>(?<value>[^>]+)</div>"));
        protected override Parser DetailsBalkonParser { get; } = new Parser((string content) => content.Contains("<li>Balkon/Terasse</li>") ? True : False);
        protected override Parser DetailsKellerParser { get; } = new Parser((string content) => content.Contains("<li>Keller</li>") ? True : False);
        protected override Parser DetailsWbsParser { get; } = null;

        protected override List<string> ParseIds(string indexContent)
        {
            var ids = base.ParseIds(indexContent);
            return ids.Where(p => p != "page").ToList();
        }

        protected override async Task<WohnungCard> ParseDetailsAsync(string detailsContent, string wohnungId)
        {
            var card = await base.ParseDetailsAsync(detailsContent, wohnungId);

            card.Wbs = card.Header.Contains("WBS", StringComparison.InvariantCultureIgnoreCase);

            card.Complete = !string.IsNullOrEmpty(card.Header) &&
                            !string.IsNullOrEmpty(card.Anschrift) &&
                            card.Balkon != null &&
                            !string.IsNullOrEmpty(card.Beschreibung) &&
                            !string.IsNullOrEmpty(card.Bezirk) &&
                            card.Etage != null &&
                            //Etagen gibt es nicht
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
