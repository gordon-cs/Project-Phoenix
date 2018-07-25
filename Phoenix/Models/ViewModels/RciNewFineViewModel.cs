namespace Phoenix.Models.ViewModels
{
    public class RciNewFineViewModel
    {
        public int RoomComponentTypeId { get; set; }
        public int RciId { get; set; }
        public string FineReason { get; set; }
        public decimal FineAmount { get; set; }
        public string FineOwner { get; set; }
    }
}