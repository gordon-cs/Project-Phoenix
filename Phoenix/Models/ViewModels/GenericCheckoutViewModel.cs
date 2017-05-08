using System;
using System.Collections.Generic;

namespace Phoenix.Models.ViewModels
{
    public class GenericCheckoutViewModel
    {
        public int RciID { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public DateTime? CheckoutSigRes { get; set; }
        public DateTime? CheckoutSigRA { get; set; }
        public DateTime? CheckoutSigRD { get; set; }
        public string CheckoutSigRAName { get; set; }
        public string CheckoutSigRAGordonID { get; set; }
        public string CheckoutSigRDName { get; set; }
        public string CheckoutSigRDGordonID { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }

    }
}