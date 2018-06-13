using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.DataAccessLayer
{
    public class RciQueries
    {
        private RCIContext Db { get; set; }

        public RciQueries()
        {
            this.Db = new RCIContext();
        }

        public RciQueries(RCIContext db)
        {
            this.Db = db;
        }

        public IEnumerable<FullRci> Rcis()
        {
            // Using a left join implemented by a group join.
            // See https://docs.microsoft.com/en-us/dotnet/csharp/linq/perform-left-outer-joins
            // For details 
            var query = from rci in this.Db.Rci
                        join account in this.Db.Account on rci.GordonID equals account.ID_NUM into rciAccount
                        from subAccount in rciAccount.DefaultIfEmpty()
                        select new FullRci
                        {
                            RciID = rci.RciID,
                            BuildingCode = rci.BuildingCode.Trim(),
                            RoomNumber = rci.RoomNumber.Trim(),
                            FirstName = subAccount == null ? "Common Area" : subAccount.firstname,
                            LastName = subAccount == null ? "RCI" : subAccount.lastname,
                            Email = subAccount.email,
                            GordonID = rci.GordonID,
                            SessionCode = rci.SessionCode,
                            LifeAndConductStatementSignature = rci.LifeAndConductSigRes,
                            CheckinSigRes = rci.CheckinSigRes,
                            CheckinSigRA = rci.CheckinSigRA,
                            CheckinSigRD = rci.CheckinSigRD,
                            CheckoutSigRes = rci.CheckoutSigRes,
                            CheckoutSigRA = rci.CheckoutSigRA,
                            CheckoutSigRD = rci.CheckoutSigRD,
                            CheckinRA = rci.CheckinSigRAGordonID,
                            CheckinRD = rci.CheckinSigRDGordonID,
                            CheckoutRA = rci.CheckoutSigRAGordonID,
                            CheckoutRD = rci.CheckoutSigRDGordonID,
                            RciComponent = rci.RciComponent,
                            CommonAreaRciSignatures = rci.CommonAreaRciSignature,
                        };

            return query;
        }
    }
}