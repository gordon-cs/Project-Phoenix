using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public class ResidentHallGrouping
    {
        public string HallGroup { get; set; }

        public List<string> BuildingCodes { get; set; }
    }
}