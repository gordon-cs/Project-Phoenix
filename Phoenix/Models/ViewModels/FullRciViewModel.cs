using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Models.ViewModels
{
    public class FullRciViewModel
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
        public string RoomNumber { get; set; }
        public string SessionCode { get; set; }
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
        public List<CommonAreaMemberViewModel> CommonAreaMembers { get; set; }
        public List<RoomComponentViewModel> RoomComponents { get; set; }
        public AccountInfoViewModel CheckinRaAccount { get; set; }
        public AccountInfoViewModel CheckinRdAccount { get; set; }
        public AccountInfoViewModel CheckoutRaAccount { get; set; }
        public AccountInfoViewModel CheckoutRdAccount { get; set; }

        public FullRciViewModel(DapperDal.Types.BigRci original)
        {
            this.RciId = original.RciId;
            this.GordonId = original.GordonId;
            this.FirstName = original.FirstName;
            this.LastName = original.LastName;
            this.Email = original.Email;
            this.AdUsername = original.AdUsername;
            this.AccountType = original.AccountType;
            this.IsCurrent = original.IsCurrent;
            this.CreationDate = original.CreationDate;
            this.BuildingCode = original.BuildingCode;
            this.RoomNumber = original.RoomNumber;
            this.SessionCode = original.SessionCode;
            this.RciTypeId = original.RciTypeId;
            this.RciTypeName = original.RciTypeName;
            this.ResidentCheckinDate = original.ResidentCheckinDate;
            this.RaCheckinDate = original.RaCheckinDate;
            this.RdCheckinDate = original.RdCheckinDate;
            this.LifeAndConductSignatureDate = original.LifeAndConductSignatureDate;
            this.ResidentCheckoutDate = original.ResidentCheckoutDate;
            this.RaCheckoutDate = original.RaCheckoutDate;
            this.RdCheckoutDate = original.RdCheckoutDate;
            this.CheckoutRaGordonId = original.CheckoutRaGordonId;
            this.CheckoutRdGordonId = original.CheckoutRdGordonId;
            this.CheckinRaGordonId = original.CheckinRaGordonId;
            this.CheckinRdGordonId = original.CheckinRdGordonId;

            this.CommonAreaMembers = original.RoomOrApartmentResidents
                .Select(x =>
                {
                    var signaturesOfThisResident = original.CommonAreaSignatures.Where(s => s.GordonId.Equals(x.GordonId));

                    return new CommonAreaMemberViewModel(signaturesOfThisResident, x);
                })
                .ToList();

            this.RoomComponents = original.RoomComponentTypes
                .Select(x =>
                {
                    var damagesToThisComponentType = original.Damages.Where(d => d.RoomComponentTypeId.Equals(x.RoomComponentTypeId));
                    var finesAddedToThisComponentType = original.Fines.Where(f => f.RoomComponentTypeId.Equals(x.RoomComponentTypeId));

                    return new RoomComponentViewModel(x, damagesToThisComponentType, finesAddedToThisComponentType);
                })
                .ToList();

            this.CheckinRaAccount = new AccountInfoViewModel(original.CheckinRaAccount);
            this.CheckinRdAccount = new AccountInfoViewModel(original.CheckinRdAccount);
            this.CheckoutRaAccount = new AccountInfoViewModel(original.CheckoutRaAccount);
            this.CheckoutRdAccount = new AccountInfoViewModel(original.CheckoutRdAccount);
        }

        public bool isViewableBy(string userID, string role, string currentRoomNumber, string currentBuilding)
        {
            // Reslife members can always view an rci
            if (role == Constants.RA || role == Constants.RD || role == Constants.ADMIN)
            {
                return true;
            }
            // If you are not a reslife member but are the owner of the rci, you can view it.
            else if (this.GordonId == userID)
            {
                return true;
            }
            // IF you are neither a reslife member or the owner of the rci, but you are part of the room to which the rci refers, you can view it.
            // i.e. Common Areas
            else if (currentRoomNumber.TrimEnd(Constants.ROOM_NUMBER_SUFFIXES) == this.RoomNumber && currentBuilding == this.BuildingCode)
            {
                return true;
            }
            // If none of the above is true, you cannot view the rci.
            else
            {
                return false;
            }
        }
    }

    public class RoomComponentViewModel
    {
        public string BuildingCode { get; set; }
        public string RciTypeName { get; set; }
        public int RoomComponentTypeId { get; set; }
        public string RoomComponentName { get; set; }
        public string RoomArea { get; set; }
        public string SuggestedCosts { get; set; }
        public List<DapperDal.Types.Damage> Damages { get; set; }
        public List<DapperDal.Types.Fine> Fines { get; set; }

        public RoomComponentViewModel(DapperDal.Types.RoomComponentType componentType, IEnumerable<DapperDal.Types.Damage> damages, IEnumerable<DapperDal.Types.Fine> fines)
        {
            this.BuildingCode = componentType.BuildingCode;
            this.RciTypeName = componentType.RciTypeName;
            this.RoomComponentTypeId = componentType.RoomComponentTypeId;
            this.RoomComponentName = componentType.RoomComponentName;
            this.RoomArea = componentType.RoomArea;
            this.SuggestedCosts = componentType.SuggestedCosts;
            this.Damages = damages.ToList();
            this.Fines = fines.ToList();
        }
    }

    public class CommonAreaMemberViewModel
    {
        public string GordonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? CheckinDate { get; set; }
        public DateTime? CheckoutDate { get; set; }

        public CommonAreaMemberViewModel(IEnumerable<DapperDal.Types.CommonAreaRciSignature> signaturesByThisMember, DapperDal.Types.Account thisMember)
        {
            this.GordonId = thisMember.GordonId;
            this.FirstName = thisMember.FirstName ?? "Alumni";
            this.LastName = thisMember.LastName ?? thisMember.GordonId;

            var checkinSignature = signaturesByThisMember.FirstOrDefault(x => x.SignatureType.Equals("CHECKIN"));
            var checkouSignature = signaturesByThisMember.FirstOrDefault(x => x.SignatureType.Equals("CHECKOUT"));

            if (checkinSignature != null)
            {
                this.CheckinDate = checkinSignature.SignatureDate;
            }

            if (checkinSignature != null)
            {
                this.CheckoutDate = checkouSignature.SignatureDate;
            }
        }
    }

    public class AccountInfoViewModel
    {
        public string GordonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdUsername { get; set; }
        public string Email { get; set; }
        public bool IsResidentAdvisor { get; set; }
        public string ResidentAdvisorBuilding { get; set; }
        public bool IsResidentDirector { get; set; }
        public string ResidentDirectorHallGroup { get; set; }
        public bool IsAdministrator { get; set; }

        public AccountInfoViewModel(DapperDal.Types.Account original)
        {
            if (original == null)
            {
                return;
            }

            this.GordonId = original.GordonId;
            this.FirstName = original.FirstName ?? "Alumni";
            this.LastName = original.LastName ?? original.GordonId;
            this.AdUsername = original.AdUsername;
            this.Email = original.Email;
            this.IsResidentAdvisor = original.IsRa;
            this.IsResidentDirector = original.IsRd;
            this.IsAdministrator = original.IsAdmin;
            this.ResidentAdvisorBuilding = original.RaBuildingCode;
            this.ResidentDirectorHallGroup = original.RdHallGroup;
        }
    }
}