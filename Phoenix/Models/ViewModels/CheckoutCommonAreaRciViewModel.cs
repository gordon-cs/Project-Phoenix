using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class CheckoutCommonAreaRciViewModel
    {
        public int RciID { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }
        public ICollection<CommonAreaMember> CommonAreaMember { get; set; }
        public DateTime? CheckoutSigRes { get; set; }
        public DateTime? CheckoutSigRA { get; set; }
        public DateTime? CheckoutSigRD { get; set; }
        public string CheckoutSigRAName { get; set; }
        public string CheckoutSigRAGordonID { get; set; }
        public string CheckoutSigRDName { get; set; }
        public string CheckoutSigRDGordonID { get; set; }

        public bool EveryoneHasSigned()
        {
            var everyoneHasSigned = true;
            foreach(var member in CommonAreaMember)
            {
                if(member.HasSignedCommonAreaRci == false)
                {
                    everyoneHasSigned = false;
                }
            }
            return everyoneHasSigned;
        }
    }
}