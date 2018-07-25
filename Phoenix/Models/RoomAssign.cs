namespace Phoenix.Models
{
    using System;

    public partial class RoomAssign
    {
        public int APPID { get; set; }
        public string SESS_CDE { get; set; }
        public string BLDG_LOC_CDE { get; set; }
        public string BLDG_CDE { get; set; }
        public string ROOM_CDE { get; set; }
        public int ROOM_SLOT_NUM { get; set; }
        public int? ID_NUM { get; set; }
        public string ROOM_TYPE { get; set; }
        public string ROOM_ASSIGN_STS { get; set; }
        public DateTime? ASSIGN_DTE { get; set; }
        public byte[] APPROWVERSION { get; set; }
        public string USER_NAME { get; set; }
        public string JOB_NAME { get; set; }
        public DateTime? JOB_TIME { get; set; }
    }
}
