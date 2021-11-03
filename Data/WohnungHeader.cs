using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class WohnungHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Provider { get; set; }

        [StringLength(256)]
        public string WohnungId { get; set; }

        public bool TraumWohnung { get; set; }
        
        public bool Gesehen { get; set; }

        public bool Gemeldet { get; set; }

        public virtual ICollection<WohnungDetails> Details { get; set; }
    }
}
