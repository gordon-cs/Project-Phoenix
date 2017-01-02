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

        public int? Fine { get; set; }

        public int RCIComponentID { get; set; }

        public virtual RCIComponent RCIComponent { get; set; }
    }
}
