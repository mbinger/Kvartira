using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Providers.Degewo
{
    public class DegewoProvider: ProviderBase
    {
        public DegewoProvider(IDownloader downloader, Log log) : base(downloader, log)
        {
        }

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

        protected override Parser DetailsHeaderParser { get; } = null;

        protected override Parser DetailsGrundMieteParser { get; } = null;

        protected override Parser DetailsGesamtMieteParser { get; } = null;

        protected override Parser DetailsKautionParser { get; } = null;

        protected override Parser DetailsAnschriftParser { get; } = null;

        protected override Parser DetailsBezirkParser { get; } = null;

        protected override Parser DetailsBeschreibungParser { get; } = null;

        protected override Parser DetailsEtageParser { get; } = null;

        protected override Parser DetailsEtagenParser { get; } = null;

        protected override Parser DetailsWbsParser { get; } = null;

        protected override Parser DetailsZimmerParser { get; } = null;

        protected override Parser DetailsFlaecheParser { get; } = null;

        protected override Parser DetailsFreiAbParser { get; } = null;

        protected override Parser DetailsBalkonParser { get; } = null;

        protected override Parser DetailsKellerParser { get; } = null;
    }
}
