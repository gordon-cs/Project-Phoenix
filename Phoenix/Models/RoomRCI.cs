namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoomRCI")]
    public partial class RoomRCI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoomRCI()
        {
            RCIComponent = new HashSet<RCIComponent>();
            ResidentRCI = new HashSet<ResidentRCI>();
        }

        public int RoomRCIID { get; set; }

        public string SessionCode { get; set; }

        [StringLength(128)]
        public string RoomID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCIComponent> RCIComponent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResidentRCI> ResidentRCI { get; set; }
    }
}
