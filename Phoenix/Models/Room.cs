using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    /// <summary>
    /// Represents a room
    /// </summary>
    public class Room
    {
        // Primary Key
        [Key]
        public string RoomID { get; set; }

        public int Capacity { get; set; }
    }
}