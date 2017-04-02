using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<string> Sessions { get; set; }
        public IEnumerable<string> Buildings { get; set; }
        public IEnumerable<string> RciTypes { get; set; }
    }
}