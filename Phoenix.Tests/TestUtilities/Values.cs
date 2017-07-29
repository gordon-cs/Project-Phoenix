using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.TestUtilities
{
    public static class Values
    {
        // Standard wait
        public static TimeSpan DEFAULT_WAIT = new TimeSpan(0, 0, 10);
        // A shorter wait
        public static TimeSpan ONE_MOMENT = new TimeSpan(0, 0, 5);

        public static string START_URL = "https://localhost:44300";

        public static string DASHBOARD_PAGE_TITLE = "Online RCI - Dashboard";
        public static string RCI_CHECKIN_PAGE_EDIT_TITLE = "Online RCI - Check-In";
        public static string RCI_CHECKIN_PAGE_REVIEW_TITLE = "Online RCI - Check-In Review";
        public static string RCI_CHECKOUT_PAGE_EDIT_TITLE = "Online RCI - Checkout";
        public static string RCI_CHECKOUT_PAGE_REVIEW_TITLE = "Online RCI - Checkout Review";
        public static string LOGIN_PAGE_TITLE = "Online RCI - Login";
    }
}
