using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    /// <summary>
    /// The RCI For a room. Contains a collection of all the RCI Components in it.
    /// Also contains a collection of all Resident RCIs associated to it.
    /// </summary>
    public class RoomRCI
    {
        public RoomRCI()
        {
            RCIComponents = new List<RCIComponent>();
            ResidentRCIs = new List<ResidentRCI>();
        }

        [Key]
        public int RoomRCIID { get; set; }

        public string SessionCode { get; set; }

        /* Foreign Key relationship to Room */
        [ForeignKey("Room")]
        public string RoomID { get; set; }
        public virtual Room Room { get; set; } // Navigational Property

        public virtual ICollection<RCIComponent> RCIComponents { get; set; }
        public virtual ICollection<ResidentRCI> ResidentRCIs { get; set; }
    }
}