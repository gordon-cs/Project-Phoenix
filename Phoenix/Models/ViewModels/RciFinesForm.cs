using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class RciFinesForm
    {
        public List<RciNewFineViewModel> NewFines { get; set; }
        public List<int> FinesToDelete { get; set; }
    }
}