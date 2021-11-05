using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class ProviderHealthEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProviderName { get; set; }

        public DateTime? LastUpdate { get; set; }

        public DateTime? IdsLoaded { get; set; }

        public DateTime? NewIdsLoaded { get; set; }

        public DateTime? DetailsLoaded { get; set; }

        public DateTime? AllDetailsComlete { get; set; }
    }
}
