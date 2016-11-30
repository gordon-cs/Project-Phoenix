using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    public class Room
    {
        [Key]
        public string RoomID { get; set; }

        public int Capacity { get; set; }
    }
}