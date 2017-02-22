namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rci")]
    public partial class Rci
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rci()
        {
            RciComponent = new HashSet<RciComponent>();
        }

        public int RciID { get; set; }

        public bool? IsCurrent { get; set; }

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

        [StringLength(50)]
        public string SessionCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckinSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckinSigRA { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckinSigRD { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LifeAndConductSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckoutSigRes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckoutSigRA { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CheckoutSigRD { get; set; }

        [StringLength(50)]
        public string CheckoutSigRAGordonID { get; set; }

        [StringLength(50)]
        public string CheckoutSigRDGordonID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RciComponent> RciComponent { get; set; }
    }
}
