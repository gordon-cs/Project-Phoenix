using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IDictionary<string, string> Sessions { get; set; }
        public IDictionary<string, string> Buildings { get; set; }
        public IEnumerable<Tuple<string, string>> RciTypes { get; set; }
        public SearchResultsViewModel SearchResults { get; set; }
    }
}