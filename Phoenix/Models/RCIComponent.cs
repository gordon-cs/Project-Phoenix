using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    public class RCIComponent
    {
        public RCIComponent()
        {
            Damages = new List<Damage>();
        }
        [Key]
        public string ComponentID { get; set; }

        public string ComponentName { get; set; }

        public virtual ICollection<Damage> Damages { get; set; }

    }
}