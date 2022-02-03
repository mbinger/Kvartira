using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class WohnungEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime Geladen { get; set; }

        [StringLength(50)]
        public string Provider { get; set; }

        [StringLength(256)]
        public string WohnungId { get; set; }

        public int Wichtigkeit { get; set; }

        [StringLength(256)]
        public string SucheShort { get; set; }

        [StringLength(1024)]
        public string SucheDetails { get; set; }

        public DateTime? Gesehen { get; set; }

        public DateTime? Gemeldet { get; set; }

        public int LoadDetailsTries { get; set; }

        [StringLength(256)]
        public string Ueberschrift { get; set; }

        [StringLength(256)]
        public string Bezirk { get; set; }

        [StringLength(512)]
        public string Anschrift { get; set; }

        public decimal? MieteKalt { get; set; }

        public decimal? MieteWarm { get; set; }

        public int? Etage { get; set; }

        public int? Etagen { get; set; }

        public int? Zimmer { get; set; }

        public decimal? Flaeche { get; set; }

        public DateTime? FreiAb { get; set; }

        [StringLength(1024)]
        public string Beschreibung { get; set; }

        public bool? Wbs { get; set; }

        public bool? Balkon { get; set; }

        public bool? Keller { get; set; }

        public bool DetailsLoaded { get; set; }
    }
}