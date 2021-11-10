using Newtonsoft.Json;
using System;

namespace Common
{
    [JsonObject("Wohnung")]
    public class WohnungExportItem
    {
        public string Provider { get; set; }
        public string WohnungId { get; set; }
        public int Wichtigkeit { get; set; }
        public string SucheShort { get; set; }
        public string SucheDetails { get; set; }
        public DateTime Geladen { get; set; }
        public DateTime? Gesehen { get; set; }
        public DateTime? Gemeldet { get; set; }
        public int LoadDetailsTries { get; set; }

        public bool Details { get; set; }
        public string Ueberschrift { get; set; }
        public string Bezirk { get; set; }
        public string Anschrift { get; set; }
        public decimal? MieteKalt { get; set; }
        public decimal? MieteWarm { get; set; }
        public int? Etage { get; set; }
        public int? Etagen { get; set; }
        public int? Zimmer { get; set; }
        public decimal? Flaeche { get; set; }
        public DateTime? FreiAb { get; set; }
        public string Beschreibung { get; set; }
        public bool? Wbs { get; set; }
        public bool? Balkon { get; set; }
        public bool? Keller { get; set; }
    }
}
