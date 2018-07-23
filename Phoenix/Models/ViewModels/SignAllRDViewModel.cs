using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class SignAllRDViewModel
    {
        public int RciID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public bool QueuedForSigning { get; set; }

        public SignAllRDViewModel(Phoenix.DapperDal.Types.Rci rci, bool isQueued)
        {
            this.RciID = rci.RciId;

            this.FirstName = rci.FirstName;

            this.LastName = rci.LastName;

            this.BuildingCode = rci.BuildingCode;

            this.RoomNumber = rci.RoomNumber;

            this.QueuedForSigning = isQueued;

            // Smooth out how the common area rcis are displayed
            if (rci.GordonId == null)
            {
                this.FirstName = "Common Area";
                this.LastName = "Rci";
            }
        }
    }
}