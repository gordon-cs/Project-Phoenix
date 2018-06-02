using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.TestUtilities
{
    public static class Credentials
    {
        /* Dorm-related people */

        // Resident in a dorm building
        // DB Requirements:
        // - Must be in the same building as DORM_RA_USERNAME (RoomAssign Table)
        public static string DORM_RES_USERNAME = "RCI-test1";
        public static string DORM_RES_PASSWORD = "Eze1820!";
        public static string DORM_RES_ID_NUMBER = "999999089";

        // Resident Advisor in a dorm building 
        // DB Requirements:
        // - Must be in charge of the buildings DORM_RD_USERNAME is in charge of. (CurrentRA Table)
        // - Must be a resident in said building. (RoomAssign Table)
        public static string DORM_RA_USERNAME = "360.studenttest";
        public static string DORM_RA_PASSWORD = "Gordon16";
        public static string DORM_RA_ID_NUMBER = "999999097";

        // Resident Director in a dorm building
        // DB Requirements:
        // - Must have an entry in the CurrentRD view.
        public static string DORM_RD_USERNAME = "360.stafftest";
        public static string DORM_RD_PASSWORD = "Gordon16";
        public static string DORM_RD_ID_NUMBER = "999999098";

        
        
        
        
        
        
        
        
        
        /* Apartment-related people */

        // Apartment residents  #1 - 4 
        // DB Requirements:
        // - Must be residents in the same building as APT_AC_USERNAME (RoomAssign Table)
        // - Must all be in the same apartment (RoomAssign Table)
        public static string APT_RES_1_USERNAME = "Rci-test2";
        public static string APT_RES_1_PASSWORD = "Eze1820!";
        public static string APT_RES_1_ID_NUMBER = "999999090";

        public static string APT_RES_2_USERNAME = "Rci-test3";
        public static string APT_RES_2_PASSWORD = "Eze1820!";
        public static string APT_RES_2_ID_NUMBER = "999999091";

        public static string APT_RES_3_USERNAME = "Rci-test4";
        public static string APT_RES_3_PASSWORD = "Eze1820!";
        public static string APT_RES_3_ID_NUMBER = "999999092";

        public static string APT_RES_4_USERNAME = "Rci-test5";
        public static string APT_RES_4_PASSWORD = "Eze1820!";
        public static string APT_RES_4_ID_NUMBER = "999999093";

        // Apartment Coordinator in an apartment building 
        // DB Requirements:
        // - Must be in charge of the buildings APT_RD_USERNAME is in charge of. (CurrentRA Table)
        // - Must be a resident in said building. (RoomAssign Table)
        public static string APT_RA_USERNAME = "Rci-test6";
        public static string APT_RA_PASSWORD = "Eze1820!";
        public static string APT_RA_ID_NUMBER = "999999094";

        // Resident Director in an apartment building
        // DB Requirements:
        // - Must have an entry in the CurrentRD view.
        public static string APT_RD_USERNAME = "Rci-test7";
        public static string APT_RD_PASSWORD = "Eze1820!";
        public static string APT_RD_ID_NUMBER = "999999095";




    }
}
