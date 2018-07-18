using Phoenix.Utilities;
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

        public HomeRciViewModel()
        {
        }

        public HomeRciViewModel(Phoenix.DapperDal.Types.Rci rci)
        {
            this.RciID = rci.RciId;
            this.BuildingCode = rci.BuildingCode.Trim();
            this.RoomNumber = rci.RoomNumber.Trim();
            this.FirstName = rci.FirstName;
            this.LastName = rci.LastName;
            this.RciStage = rci.RdCheckinDate == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE;
            this.CheckinSigRes = rci.ResidentCheckinDate;
            this.CheckinSigRA = rci.RaCheckinDate;
            this.CheckinSigRD = rci.RdCheckinDate;
            this.CheckoutSigRes = rci.ResidentCheckoutDate;
            this.CheckoutSigRA = rci.RaCheckoutDate;
            this.CheckoutSigRD = rci.RdCheckoutDate;

            // Smooth out Common Area Rcis
            // Common Area Rcis lack a gordonId
            if (string.IsNullOrWhiteSpace(rci.GordonId))
            {
                this.FirstName = "Common Area";
                this.LastName = "Rci";
            }

            // Smooth out Rcis for Rcis for alumni.
            // Once graduated, students are removed from the Accounts view.
            // We can identify such Rcis because they have a GordonId, but no first or last name
            var isAlumni = !string.IsNullOrWhiteSpace(rci.GordonId) &&
                string.IsNullOrWhiteSpace(rci.FirstName) &&
                string.IsNullOrWhiteSpace(rci.LastName);

            if (isAlumni)
            {
                this.FirstName = "Unidentified User";
                this.LastName = rci.GordonId;
            }
        }

    }
}