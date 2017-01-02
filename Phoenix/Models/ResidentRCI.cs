namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ResidentRCI")]
    public partial class ResidentRCI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ResidentRCI()
        {
            RCIComponent = new HashSet<RCIComponent>();
        }

        public int ResidentRCIID { get; set; }

        public string SessionCode { get; set; }

        public int? RoomRCIID { get; set; }

        [Required]
        public string ResidentAccountID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCIComponent> RCIComponent { get; set; }

        public virtual RoomRCI RoomRCI { get; set; }
    }
}
