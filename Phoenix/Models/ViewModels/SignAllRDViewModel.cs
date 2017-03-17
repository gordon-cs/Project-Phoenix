using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class SignAllRDViewModel
    {
        public int RciID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string CheckinSigRDGordonID { get; set; }
    }
}