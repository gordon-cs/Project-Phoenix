namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RCI")]
    public partial class RCI
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RCIID { get; set; }

        public bool? Current { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(50)]
        public string BuildingCode { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; }

        [StringLength(50)]
        public string GordonID { get; set; }
    }
}
