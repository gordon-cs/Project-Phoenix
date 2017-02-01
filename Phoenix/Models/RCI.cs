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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RCI()
        {
            RCIComponent = new HashSet<RCIComponent>();
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCIComponent> RCIComponent { get; set; }
    }
}
