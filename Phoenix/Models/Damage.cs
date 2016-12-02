using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    /// <summary>
    /// A damage entry specifies the component that was damaged and the corresponding fine. Multiple damages can be recorded for one component
    /// </summary>
    public class Damage
    {
        // Primary Key
        [Key]
        public string DamageID { get; set; }

        public string DamageDescription { get; set; }

        public int Fine { get; set; }

        /* Foreign Key Relationship with RCIComponent */
        [ForeignKey("RCIComponent")]
        public string RCIComponentID { get; set; }
        public virtual RCIComponent RCIComponent { get; set; } // Navigational Property
      
    }
}