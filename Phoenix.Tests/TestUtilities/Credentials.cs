using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.TestUtilities
{
    public static class Credentials
    {
        // Resident in a dorm building
        // DB Requirements:
        // - Must be in the same building as RA_USERNAME
        public static string RES_USERNAME = "RCI-test1";
        public static string DORM_RES_PASSWORD = "NwNiEPS6";
        public static string DORM_RES_ID_NUMBER = "";
        // Resident Advisor in a dorm building 
        // DB Requirements:
        // - Must be in charge of the buildings RD_USERNAME is in charge of.
        // - Must be a resident in said building.
        public static string RA_USERNAME = "360.studenttest";
        public static string RA_PASSWORD = "Gordon16";
        public static string RA_ID_NUMBER = "999999097";

        // Resident Director in a dorm building
        // DB Requirements:
        // - Must have an entry in the Current_RD view.
        public static string RD_USERNAME = "360.stafftest";
        public static string RD_PASSWORD = "Gordon16";
        public static string RD_ID_NUMBER = "999999098";
    }
}
