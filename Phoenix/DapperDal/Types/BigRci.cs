using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public class BigRci : Rci
    {
        public List<CommonAreaRciSignature> CommonAreaSignatures { get; set; }
        public List<RoomComponentType> RoomComponentTypes { get; set; }
        public List<Damage> Damages { get; set; }
        public List<Fine> Fines { get; set; }
        public List<Account> RoomOrApartmentResidents { get; set; }
        public Account CheckinRaAccount { get; set; }
        public Account CheckinRdAccount { get; set; }
        public Account CheckoutRaAccount { get; set; }
        public Account CheckoutRdAccount { get; set; }
    }
}