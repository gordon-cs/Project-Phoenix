using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    public class Damage
    {
        [Key]
        public string DamageID { get; set; }

        public string DamageDescription { get; set; }
    }
}