namespace Phoenix.Utilities
{
    public static class Constants
    {
        public const string RESIDENT = "Resident";
        public const string RA = "RA";
        public const string RD = "RD";
        public const string ADMIN = "ADMIN";

        public const string RCI_UNSIGNED = "RCI_UNSIGNED";
        public const string RCI_SIGNGED_BY_RES_CHECKIN = "RCI_SIGNGED_BY_RES_CHECKIN";
        public const string RCI_SIGNGED_BY_RA_CHECKIN = "RCI_SIGNGED_BY_RA_CHECKIN";
        public const string RCI_SIGNGED_BY_RD_CHECKIN = "RCI_SIGNGED_BY_RD_CHECKIN";
        public const string RCI_SIGNGED_BY_RES_CHECKOUT = "RCI_SIGNGED_BY_RES_CHECKOUT";
        public const string RCI_SIGNGED_BY_RA_CHECKOUT = "RCI_SIGNGED_BY_RA_CHECKOUT";
        public const string RCI_COMPLETE = "RCI_COMPLETE";

        public const string RCI_CHECKIN_STAGE = "CHECKIN";
        public const string RCI_CHECKOUT_STAGE = "CHECKOUT";

        public const string WORK_REQUEST_ENDPOINT = "/departments/physicalplant/ppwos/add_reqform.cfm";
        public const string GO_GORDON_URL = "https://go.gordon.edu";

        public const string FINE = "Fine";
        public const string RCI_TYPE_COMMON = "common";
        public const string RCI_TYPE_INDIVIDUAL = "individual";

        public static readonly char[] ROOM_NUMBER_SUFFIXES = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
    }
}