using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class CheckinIndividualRoomRciViewModel
    {
        public int RciID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string GordonID { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }
        public DateTime? CheckinSigRes { get; set; }
        public DateTime? CheckinSigRA { get; set; }
        public DateTime? CheckinSigRD { get; set; }
        public string CheckinSigRAName { get; set; }
        public string CheckinSigRAGordonID { get; set; }
        public string CheckinSigRDName { get; set; }
        public string CheckinSigRDGordonID { get; set; }

        public bool DamagesExist()
        {
            return RciComponent.Where(x => x.Damage.Any()).Any();
        }
    }
}