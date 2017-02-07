namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fine")]
    public partial class Fine
    {
        public int fineID { get; set; }

        public decimal fineAmount { get; set; }

        [StringLength(50)]
        public string gordonID { get; set; }

        [Required]
        public string reason { get; set; }

        public int? rciComponentID { get; set; }

        public virtual RCIComponent RCIComponent { get; set; }
    }
}
