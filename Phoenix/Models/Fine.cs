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
        public int FineID { get; set; }

        public decimal FineAmount { get; set; }

        [StringLength(50)]
        public string GordonID { get; set; }

        [Required]
        public string Reason { get; set; }

        public int? RciComponentID { get; set; }

        public virtual RciComponent RciComponent { get; set; }
    }
}
