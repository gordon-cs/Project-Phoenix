namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BuildingAssign")]
    public partial class BuildingAssign
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string JobTitleHall { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string BuildingCode { get; set; }

        [StringLength(50)]
        public string GordonID { get; set; }

        [StringLength(100)]
        public string RDName { get; set; }
    }
}
