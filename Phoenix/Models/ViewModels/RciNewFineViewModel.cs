using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class RciNewFineViewModel
    {
        public int ComponentID { get; set; }
        public string FineReason { get; set; }
        public decimal FineAmount { get; set; }
        public string FineOwner { get; set; }
    }
}