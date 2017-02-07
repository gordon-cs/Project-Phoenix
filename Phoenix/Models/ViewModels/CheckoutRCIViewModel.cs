using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class CheckoutRCIViewModel
    {
        public int rciID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string buildingCode { get; set; }
        public string roomNumber { get; set; }
        public string gordonID { get; set; }
        public ICollection<RCIComponent> rciComponents { get; set; }
    }
}