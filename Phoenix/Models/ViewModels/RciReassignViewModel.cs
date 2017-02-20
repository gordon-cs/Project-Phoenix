using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class RciReassignViewModel
    {
        public int RciID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string GordonID { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }
        public ICollection<PotentialRciReassignTarget> RciTarget { get; set; }
    }
}