namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Damage")]
    public partial class Damage
    {
        public int damageID { get; set; }

        public string damageDescription { get; set; }

        public string damageImagePath { get; set; }

        [Required]
        [StringLength(50)]
        public string damageType { get; set; }

        public int rciComponentID { get; set; }

        public decimal? fineAssessed { get; set; }

        public virtual RCIComponent RCIComponent { get; set; }
    }
}
