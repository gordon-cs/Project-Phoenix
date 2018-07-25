namespace Phoenix.DapperDal.Types
{
    public class Fine
    {
        public int FineId { get; set; }
        public decimal Amount { get; set; }
        public string GordonId { get; set; }
        public string Reason { get; set; }
        public int RoomComponentTypeId { get; set; }
        public int RciId { get; set; }
    }
}
