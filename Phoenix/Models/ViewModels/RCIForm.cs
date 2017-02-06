using System.Collections.Generic;

namespace Phoenix.Models.ViewModels
{
    public class RCIForm
    {
        public List<RCINewDamageViewModel> newDamages { get; set; }
        public List<int> damagesToDelete { get; set; }
    }
}