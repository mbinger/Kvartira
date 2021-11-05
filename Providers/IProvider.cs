using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Providers
{
    public class WohnungCard
    {
        public string Header { get; set; }

        public string Bezirk { get; set; }

        public string Anschrift { get; set; }

        public decimal? MieteKalt { get; set; }

        public decimal? MieteWarm { get; set; }

        public decimal? Kaution { get; set; }

        public int? Etage { get; set; }

        public int? Etagen { get; set; }

        public int? Zimmer { get; set; }

        public decimal? Flaeche { get; set; }

        public DateTime? FreiAb { get; set; }

        public string Beschreibung { get; set; }

        public bool? Wbs { get; set; }

        public bool? Balkon { get; set; }

        public bool? Keller { get; set; }

        /// <summary>
        /// WohnungCard complete
        /// </summary>
        public bool Complete { get; set; }
    }

    public class LoadIdsResult
    {
        public int PagesCount { get; set; }
        public List<string> WohnungIds { get; set; } = new List<string>();
    }

    public interface IProvider
    {
        public string Name { get; }
        public Task<LoadIdsResult> LoadIndexAsync(Search config);
        public Task<WohnungCard> LoadDetailsAsync(string wohnungId);
        public string GetOpenDetailsUrl(string wohnungId);
    }
}
