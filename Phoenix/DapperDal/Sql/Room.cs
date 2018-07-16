namespace Phoenix.DapperDal.Sql
{
    public class Room
    {
        public const string RoomSelectStatement =
            @"select room.BLDG_CDE as BuildingCode,
		room.BUILDING_DESC as BuildingDescription,
		room.ROOM_CDE as RoomNumber,
		room.ROOM_TYPE as RoomType
from Room room
";
    }
}