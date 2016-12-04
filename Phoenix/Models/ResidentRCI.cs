using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    /// <summary>
    /// The RCI For a resident. It is made up of a collection of RCIComponents
    /// </summary>
    public class ResidentRCI
    {
        public ResidentRCI()
        {
            RCIComponents = new List<RCIComponent>();
        }

        // Primary key
        [Key]
        public int ResidentRCIID { get; set; }

        public string SessionCode { get; set; }

        /* Foreign key relationship to the related RoomRCI */
        public int RoomRCIID { get; set; }
        public virtual RoomRCI RoomRCI { get; set; }

        /* Foreign Key relationship to the resident's account */
        public string ResidentAccountID { get; set; }

        public virtual ICollection<RCIComponent> RCIComponents { get; set; }
    }
}