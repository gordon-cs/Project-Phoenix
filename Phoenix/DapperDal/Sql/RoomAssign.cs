namespace Phoenix.DapperDal.Sql
{
    public class RoomAssign
    {
        public const string RoomAssignmentSelectStatment =
            @"select ID_NUM as GordonId,
		RTRIM(BLDG_CDE) as BuildingCode,
		RTRIM(ROOM_CDE) as RoomNumber,
		ROOM_TYPE as RoomType,
		RTRIM(SESS_CDE) as SessionCode,
		ASSIGN_DTE as AssignmentDate
from RoomAssign
";
    }
}