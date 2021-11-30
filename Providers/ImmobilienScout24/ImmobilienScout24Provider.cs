using Common;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Providers.ImmobilienScout24
{
    public class ImmobilienScout24Provider : ProviderBase
    {
        public ImmobilienScout24Provider(BrowserDownloader downloader, ILog log) : base(downloader, log)
        {
        }

        public override string Name { get; } = "IMMO SCOUT 24";

        protected override string DetailsUrl { get; } = "https://www.immobilienscout24.de/expose/{0}#/";

        protected override Parser NichtsGefundenParser { get; } = new Parser((content) => content.Contains("Leider haben wir aktuell keine Treffer für Ihre Suchkriterien!") ? True : False);

        public override async Task<LoadIdsResult> LoadIndexAsync(Search search)
        {
            await downloader.GetAsync("https://www.immobilienscout24.de/", false,  "immobilien scout start", true);
            return await base.LoadIndexAsync(search);
        }

        protected override Regex GetIdsRegex { get; } = new Regex("data-id=\"(?<value>\\d+)\"");

        protected override Regex IndexNextPageUrlRegex => throw new NotImplementedException();

        protected override Parser DetailsHeaderParser => throw new NotImplementedException();

        protected override Parser DetailsGrundMieteParser => throw new NotImplementedException();

        protected override Parser DetailsGesamtMieteParser => throw new NotImplementedException();

        protected override Parser DetailsKautionParser => throw new NotImplementedException();

        protected override Parser DetailsAnschriftParser => throw new NotImplementedException();

        protected override Parser DetailsBezirkParser => throw new NotImplementedException();

        protected override Parser DetailsBeschreibungParser => throw new NotImplementedException();

        protected override Parser DetailsEtageParser => throw new NotImplementedException();

        protected override Parser DetailsEtagenParser => throw new NotImplementedException();

        protected override Parser DetailsWbsParser => throw new NotImplementedException();

        protected override Parser DetailsZimmerParser => throw new NotImplementedException();

        protected override Parser DetailsFlaecheParser => throw new NotImplementedException();

        protected override Parser DetailsFreiAbParser => throw new NotImplementedException();

        protected override Parser DetailsBalkonParser => throw new NotImplementedException();

        protected override Parser DetailsKellerParser => throw new NotImplementedException();
    }
}
