namespace Phoenix.DapperDal.Types
{
    public class Damage
    {
        public int DamageId { get; set; }
        public string GordonId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string DamageType { get; set; }
        public int RoomComponentTypeId { get; set; }
        public int RciId { get; set; }
    }
}
