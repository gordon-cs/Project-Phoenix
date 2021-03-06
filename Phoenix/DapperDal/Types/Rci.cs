﻿using Phoenix.Utilities;
using System;
using System.Collections.Generic;

namespace Phoenix.DapperDal.Types
{
    public abstract class Rci
    {
        public int RciId { get; set; }
        public string GordonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AdUsername { get; set; }
        public string AccountType { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreationDate { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingDescription { get; set; }
        public string RoomNumber { get; set; }
        public string SessionCode { get; set; }
        public string SessionDescription { get; set; }
        public int RciTypeId { get; set; }
        public string RciTypeName { get; set; }
        public DateTime? ResidentCheckinDate { get; set; }
        public DateTime? RaCheckinDate { get; set; }
        public DateTime? RdCheckinDate { get; set; }
        public DateTime? LifeAndConductSignatureDate { get; set; }
        public DateTime? ResidentCheckoutDate { get; set; }
        public DateTime? RaCheckoutDate { get; set; }
        public DateTime? RdCheckoutDate { get; set; }
        public string CheckoutRaGordonId { get; set; }
        public string CheckoutRdGordonId { get; set; }
        public string CheckinRaGordonId { get; set; }
        public string CheckinRdGordonId { get; set; }
    }
}