using System;

namespace Phoenix.DapperDal.Types
{
    public class Session
    {
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public DateTime? SessionStartDate { get; set; }
        public DateTime? SessionEndDate { get; set; }
    }
}