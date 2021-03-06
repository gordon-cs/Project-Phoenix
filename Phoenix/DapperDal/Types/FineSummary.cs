﻿namespace Phoenix.DapperDal.Types
{
    public class FineSummary
    {
        public int FineId { get; set; }
        public bool IsCurrent { get; set; }
        public string BuildingCode { get; set; }
        public string RoomNumber { get; set; }
        public string SessionCode { get; set; }
        public string RoomComponentName { get; set; }
        public string RoomArea { get; set; }
        public string SuggestedCostsString { get; set; }
        public decimal FineAmount { get; set; }
        public string GordonId { get; set; }
        public string Reason { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}