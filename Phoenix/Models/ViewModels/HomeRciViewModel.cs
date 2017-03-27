using System;

namespace Phoenix.Models.ViewModels
{
    public class HomeRciViewModel
    {
        public int RciID { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RciStage { get; set; }
        public DateTime? CheckinSigRes { get; set; }
        public DateTime? CheckinSigRA { get; set; }
        public DateTime? CheckinSigRD { get; set; }
        public DateTime? CheckoutSigRes { get; set; }
        public DateTime? CheckoutSigRA { get; set; }
        public DateTime? CheckoutSigRD { get; set; }

    }
}