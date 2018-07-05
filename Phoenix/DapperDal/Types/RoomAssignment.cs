using System;

namespace Phoenix.DapperDal.Types
{
    public class RoomAssignment
    {
        public string GordonId { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string SessionCode { get; set; }
        public DateTime? AssignmentDate { get; set; }
    }
}