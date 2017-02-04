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
        public int DamageID { get; set; }

        public string DamageDescription { get; set; }

        public string DamageImagePath { get; set; }

        [Required]
        [StringLength(50)]
        public string DamageType { get; set; }

        public int RCIComponentID { get; set; }

        public decimal? FineAssessed { get; set; }

        public virtual RCIComponent RCIComponent { get; set; }
    }
}
