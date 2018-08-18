namespace Phoenix.DapperDal.Sql
{
    public class Room
    {
        public const string RoomSelectStatement =
            @"
select RTRIM(room.BLDG_CDE) as BuildingCode,
		RTRIM(room.BUILDING_DESC) as BuildingDescription,
		RTRIM(room.ROOM_CDE) as RoomNumber,
		room.ROOM_TYPE as RoomType
from Room room
";

        public const string BuildingCodeToBuildingMapSelectStatement =
            @"
select RTRIM(BLDG_CDE) as BuildingCode, RTRIM(BUILDING_DESC) as BuildingDescription
from Room 
group by BLDG_CDE, BUILDING_DESC
";
    }
}