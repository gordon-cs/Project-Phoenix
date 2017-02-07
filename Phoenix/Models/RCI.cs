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

        public int rciID { get; set; }

        public bool? isCurrent { get; set; }

        [Column(TypeName = "date")]
        public DateTime creationDate { get; set; }

        [Required]
        [StringLength(50)]
        public string buildingCode { get; set; }

        [Required]
        [StringLength(50)]
        public string roomNumber { get; set; }

        [StringLength(50)]
        public string gordonID { get; set; }

        [StringLength(50)]
        public string sessionCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkinSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkinSigRA { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkinSigRD { get; set; }

        [Column(TypeName = "date")]
        public DateTime? lifeAndConductSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkoutSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkoutSigRA { get; set; }

        [Column(TypeName = "date")]
        public DateTime? checkoutSigRD { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCIComponent> RCIComponent { get; set; }
    }
}
