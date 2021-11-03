using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class WohnungDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("WohnungHeader")]
        public int WohnungHeaderId { get; set; }
        public virtual WohnungHeader WohnungHeader { get; set; }

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
    }
}
