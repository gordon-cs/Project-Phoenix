namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RCIComponent")]
    public partial class RCIComponent
    {
        public int RCIComponentID { get; set; }

        [Required]
        [StringLength(50)]
        public string RCIComponentName { get; set; }

        public int RCIID { get; set; }
    }
}
