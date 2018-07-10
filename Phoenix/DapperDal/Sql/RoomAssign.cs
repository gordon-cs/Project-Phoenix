namespace Phoenix.DapperDal.Sql
{
    public class RoomAssign
    {
        public const string RoomAssignmentSelectStatment =
            @"select ID_NUM as GordonId,
		BLDG_CDE as BuildingCode,
		ROOM_CDE as RoomNumber,
		ROOM_TYPE as RoomType,
		SESS_CDE as SessionCode,
		ASSIGN_DTE as AssignmentDate
from RoomAssign
";
    }
}