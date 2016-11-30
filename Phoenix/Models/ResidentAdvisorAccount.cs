using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Phoenix.Models
{
    /// <summary>
    /// Represents a Resident Advisor account, which is really just a normal account, but with more priviledges.
    /// </summary>
    public class ResidentAdvisorAccount
    {
        /* Primary key that is also a foreign key. */
        [Key, ForeignKey("ResidentAccount")]
        public string ResidentAccountID { get; set; }
        public virtual ResidentAccount ResidentAccount { get; set; } // Navigational Property
        
    }
}