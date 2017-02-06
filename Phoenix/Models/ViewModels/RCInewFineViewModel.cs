using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class RCInewFineViewModel
    {
        public int componentId { get; set; }
        public string fineReason { get; set; }
        public decimal fineAmount { get; set; }
    }
}