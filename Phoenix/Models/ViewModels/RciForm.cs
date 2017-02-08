using System.Collections.Generic;

namespace Phoenix.Models.ViewModels
{
    public class RciForm
    {
        public List<RciNewDamageViewModel> NewDamages { get; set; }
        public List<int> DamagesToDelete { get; set; }
    }
}