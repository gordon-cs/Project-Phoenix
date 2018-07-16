namespace Phoenix.DapperDal.Sql
{
    public class Damage
    {
        public const string SimpleDamageSelectStatement =
            @"select damage.DamageId as DamageId,
		damage.DamageDescription as [Description],
		damage.DamageImagePath as ImagePath,
		damage.DamageType as DamageType,
		damage.RciId as RciId,
		damage.RoomComponentTypeId as RoomComponentTypeId,
		damage.GordonId as GordonId
from Damage damage
";

    }
}