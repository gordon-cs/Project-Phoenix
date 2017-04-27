using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class SearchResultsViewModel
    {
        public IEnumerable<HomeRciViewModel> RciSearchResult { get; set; }
    }
}