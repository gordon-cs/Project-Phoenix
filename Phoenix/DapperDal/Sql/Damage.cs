namespace Phoenix.DapperDal.Sql
{
    public class Damage
    {
        public const string DamageInsertStatement =
            @"insert into Damage(DamageDescription, DamageImagePath, DamageType, RciId, GordonId, RoomComponentTypeId)
values(@DamageDescription, @DamageImagepath, @DamageType, @RciId, @GordonId, @RoomComponentTypeId)
SELECT CAST(SCOPE_IDENTITY() as int)
";

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