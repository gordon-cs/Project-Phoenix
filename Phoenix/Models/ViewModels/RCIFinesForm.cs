using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class RCIFinesForm
    {
        public string gordonID { get; set; }
        public List<RCInewFineViewModel> newFines { get; set; }
        public List<int> finesToDelete { get; set; }
    }
}