using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public class RciComponent
    {
        public int RciComponentId { get; set; }
        public string RciComponentName { get; set; }
        public string RciComponentDescription { get; set; }
        public string SuggestedCosts { get; set; }
        public IList<Damage> Damages { get; set; }
        public IList<Fine> Fines { get; set; }
    }
}