using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public class BigRci : Rci
    {
        public IList<RoomComponentType> RoomComponentTypes { get; set; }
        public List<Damage> Damages { get; set; }
        public List<Fine> Fines { get; set; }
    }
}