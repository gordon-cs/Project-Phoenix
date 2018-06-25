using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public class BigRci : Rci
    {
        public IList<CommonAreaRciSignature> CommonAreaSignatures { get; set; }
        public IList<RciComponent> RciComponents { get; set; }
    }
}