using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    public class RoomRCI
    {
        [Key]
        public string RoomRCIID { get; set; }

        public string SessionCode { get; set; }

        [ForeignKey("Room")]
        public string RoomID { get; set; }

        public virtual Room Room { get; set; }
    }
}