using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    /// <summary>
    /// Represents a piece of the room whose state can be documented e.g. Table, chaire, wall, carpet... A component can have multiple damages listed
    /// </summary>
    public class RCIComponent
    {
        public RCIComponent()
        {
            Damages = new List<Damage>();
        }
        
        // Primary Key
        [Key]
        public int RCIComponentID { get; set; }

        public string RCIComponentName { get; set; }

        /* Foreign Key To RoomRCI */
        [ForeignKey("RoomRCI")]
        public int? RoomRCIID { get; set; }
        public virtual RoomRCI RoomRCI { get; set; }

        /* Foreign Key To ResidentRCI */
        [ForeignKey("ResidentRCI")]
        public int? ResidentRCIID { get; set; }
        public virtual ResidentRCI ResidentRCI { get; set; }

        public virtual ICollection<Damage> Damages { get; set; } // Collection of Damages

    }
}