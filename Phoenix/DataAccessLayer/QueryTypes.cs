using Phoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.DataAccessLayer
{
    public class FullRci
    {
        public int RciID { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string GordonID { get; set; }
        public string SessionCode { get; set; }
        public DateTime? LifeAndConductStatementSignature { get; set; }
        public DateTime? CheckinSigRes { get; set; }
        public DateTime? CheckinSigRA { get; set; }
        public DateTime? CheckinSigRD { get; set; }
        public DateTime? CheckoutSigRes { get; set; }
        public DateTime? CheckoutSigRA { get; set; }
        public DateTime? CheckoutSigRD { get; set; }
        public string CheckinRA { get; set; }
        public string CheckoutRA { get; set; }
        public string CheckinRD { get; set; }
        public string CheckoutRD { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }
        public ICollection<CommonAreaRciSignature> CommonAreaRciSignatures { get; set;}
    }
}