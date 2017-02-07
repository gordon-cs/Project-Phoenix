namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RciComponent")]
    public partial class RciComponent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RciComponent()
        {
            Damage = new HashSet<Damage>();
            Fine = new HashSet<Fine>();
        }

        public int RciComponentID { get; set; }

        [Required]
        [StringLength(50)]
        public string RciComponentName { get; set; }

        public int RciID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Damage> Damage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fine> Fine { get; set; }

        public virtual Rci Rci { get; set; }
    }
}
