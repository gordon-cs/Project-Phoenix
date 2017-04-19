using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Models.ViewModels
{
    /// <summary>
    /// Used to store the information that is to be displayed during checkin on the signature page of a common are rci.
    /// </summary>
    public class CheckinCommonAreaRciViewModel
    {
        public int RciID { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public ICollection<RciComponent> RciComponent { get; set; }
        public ICollection<CommonAreaMember> CommonAreaMember { get; set; }
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

        public bool EveryoneHasSigned()
        {
            var everyoneHasSigned = true;
            foreach (var member in CommonAreaMember)
            {
                if (member.HasSignedCommonAreaRci == false)
                {
                    everyoneHasSigned = false;
                }
            }
            return everyoneHasSigned;
        }
    }
}