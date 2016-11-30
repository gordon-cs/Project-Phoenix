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
        public string ResidentRCIID { get; set; }

        public string SessionCode { get; set; }

        /* Foreign Key relationship to the resident's account */
        [ForeignKey("ResidentAccount")]
        public string ResidentAccountID { get; set; }
        public virtual ResidentAccount ResidentAccount { get; set; } // Navigational property


        public virtual ICollection<RCIComponent> RCIComponents { get; set; }
    }
}