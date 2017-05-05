namespace Phoenix.Utilities
{
    public static class Constants
    {
        public static readonly string RESIDENT = "Resident";
        public static readonly string RA = "RA";
        public static readonly string RD = "RD";
        public static readonly string ADMIN = "ADMIN";

        public const string RCI_UNSIGNED = "RCI_UNSIGNED";
        public const string RCI_SIGNGED_BY_RES_CHECKIN = "RCI_SIGNGED_BY_RES_CHECKIN";
        public const string RCI_SIGNGED_BY_RA_CHECKIN = "RCI_SIGNGED_BY_RA_CHECKIN";
        public const string RCI_SIGNGED_BY_RD_CHECKIN = "RCI_SIGNGED_BY_RD_CHECKIN";
        public const string RCI_SIGNGED_BY_RES_CHECKOUT = "RCI_SIGNGED_BY_RES_CHECKOUT";
        public const string RCI_SIGNGED_BY_RA_CHECKOUT = "RCI_SIGNGED_BY_RA_CHECKOUT";
        public const string RCI_COMPLETE = "RCI_COMPLETE";

        public static readonly string RCI_CHECKIN_STAGE = "CHECKIN";
        public static readonly string RCI_CHECKOUT_STAGE = "CHECKOUT";

        public static string WORK_REQUEST_ENDPOINT = "/departments/physicalplant/ppwos/add_reqform.cfm";
        public static string GO_GORDON_URL = "https://go.gordon.edu";

        public static readonly string FINE = "Fine";
        public const string RCI_TYPE_COMMON = "common";
        public const string RCI_TYPE_INDIVIDUAL = "individual";
    }
}