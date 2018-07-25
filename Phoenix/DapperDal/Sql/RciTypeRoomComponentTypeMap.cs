namespace Phoenix.DapperDal.Sql
{
    public class RciTypeRoomComponentTypeMap
    {
        public const string MapSelectStatment =
            @"select map.BuildingCode,
		rciType.RciTypeName,
		roomComponentType.RoomComponentTypeId,
		roomComponentType.RoomComponentName,
		roomComponentType.RoomArea,
		roomComponentType.SuggestedCosts
from Rci rci
inner join RciTypeRoomComponentTypeMap map
on rci.RciTypeId = map.RciTypeId and rci.BuildingCode = map.BuildingCode
inner join RoomComponentType roomComponentType
on map.RoomComponentTypeId = roomComponentType.RoomComponentTypeId
inner join RciType rciType
on map.RciTypeId = rciType.RciTypeId
";
    }
}