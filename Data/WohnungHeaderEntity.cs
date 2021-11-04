using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class WohnungHeaderEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public virtual ICollection<WohnungDetailsEntity> Details { get; set; } = new Collection<WohnungDetailsEntity>();
    }
}
