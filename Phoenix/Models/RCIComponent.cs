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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RCIComponent()
        {
            Damage = new HashSet<Damage>();
        }

        public int RCIComponentID { get; set; }

        public string RCIComponentName { get; set; }

        public int? RoomRCIID { get; set; }

        public int? ResidentRCIID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Damage> Damage { get; set; }

        public virtual ResidentRCI ResidentRCI { get; set; }

        public virtual RoomRCI RoomRCI { get; set; }
    }
}
