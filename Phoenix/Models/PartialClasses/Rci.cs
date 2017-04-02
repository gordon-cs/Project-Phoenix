using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models
{
    public partial class Rci
    {
        public bool isViewableBy(string userID, string role, string currentRoomNumber, string currentBuilding)
        {
            if (role == Constants.RA || role == Constants.RD || role == Constants.ADMIN)
            {
                return true;
            }
            else if (GordonID == userID)
            {
                return true;
            }
            else if (currentRoomNumber.TrimEnd(new char[] { 'A', 'B', 'C', 'D' }) == RoomNumber && currentBuilding == BuildingCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsIndividualRci()
        {
            return GordonID != null ? true : false;
        }
    }
}