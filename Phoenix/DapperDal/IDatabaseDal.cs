using Phoenix.DapperDal.Types;
using System;
using System.Collections.Generic;

namespace Phoenix.DapperDal
{
    public interface IDatabaseDal
    {
        /// <summary>
        /// Create a new Dorm Room Rci 
        /// </summary>
        int CreateNewDormRci(string gordonId, string buildingCode, string roomNumber, string sessionCode);

        /// <summary>
        /// Create a new Common Area Rci
        /// </summary>
        int CreateNewCommonAreaRci(string buildingCode, string roomNumber, string sessionCode);

        /// <summary>
        /// Set the value of the IsCurrent bool column
        /// </summary>
        void SetRciIsCurrentColumn(IEnumerable<int> rciIds, bool isCurrent);

        /// <summary>
        /// Set the value of the GordonId column.
        /// </summary>
        void SetRciGordonIdColumn(IEnumerable<int> rciIds, string gordonId);

        /// <summary>
        /// Set the value of the checkin date columns. If a value is null, it is not set.
        /// </summary>
        void SetRciCheckinDateColumns(IEnumerable<int> rciIds, DateTime? residentCheckinDate, DateTime? raCheckinDate, DateTime? rdCheckinDate, DateTime? lifeAndConductStatementCheckinDate);

        /// <summary>
        /// Set the value of the checkout date columns. If a value is null, it is not set.
        /// </summary>
        void SetRciCheckoutDateColumns(IEnumerable<int> rciIds, DateTime? residentCheckoutDate, DateTime? raCheckoutDate, DateTime? rdCheckoutDate);

        /// <summary>
        /// Set the value of the checkin gordon id columns. If a values is null, it is not set.
        /// </summary>
        void SetRciCheckinGordonIdColumns(IEnumerable<int> rciIds, string checkinRaGordonId, string checkingRdGordonId);

        /// <summary>
        /// Set the value of the checkout gordon id columsn. If a value is null, it is not set.
        /// </summary>
        void SetRciCheckoutGordonIdColumns(IEnumerable<int> rciIds, string checkoutRaGordonId, string checoutRdGordonId);

        /// <summary>
        /// Delete the rci with the id provided
        /// </summary>
        void DeleteRci(int rciId);

        /// <summary>
        /// Fetch damage record
        /// </summary>
        Damage FetchDamageById(int damageId);

        /// <summary>
        /// Create a new Damage Record
        /// </summary>
        int CreateNewDamage(string description, string imagepath, int rciId, string gordonId, int roomComponentTypeId);

        /// <summary>
        /// Update a damage record. If a value is null, it is not set
        /// </summary>
        void UpdateDamage(int damageId, string description, string imagepath, int? rciId, int? roomComponentTypeId);

        /// <summary>
        /// Delete a damage record. 
        /// </summary>
        /// <param name="damageId"></param>
        void DeleteDamage(int damageId);

        /// <summary>
        /// Fetch Fine Record
        /// </summary>
        Fine FetchFineById(int fineId);

        /// <summary>
        /// Create a new fine record
        /// </summary>
        int CreateNewFine(decimal amount, string gordonId, string reason, int rciId, int roomComponentTypeId);

        /// <summary>
        /// Update Fine record. Fields left null will not be updated.
        /// </summary>
        void UpdateFine(int fineId, decimal? amount, string gordonId, string reason, int? rciId, int? roomComponentTypeId);

        /// <summary>
        /// Delete Fine record
        /// </summary>
        void DeleteFine(int fineId);

        /// <summary>
        /// Create a new common are signature record
        /// </summary>
        CommonAreaRciSignature CreateNewCommonAreaRciSignature(string gordonId, int rciId, DateTime signatureDate, string signatureType);

        /// <summary>
        /// Delete a common are rci signature record. 
        /// </summary>
        void DeleteCommonAreaRciSignature(string gordonId, int rciId, string signatureType);


        BigRci FetchRciById(int rciId);
        List<SmolRci> FetchRcisById(IEnumerable<int> rciIds);
        List<SmolRci> FetchRcisByGordonId(string gordonId);
        List<SmolRci> FetchRcisByBuilding(IEnumerable<string> buildings);
        List<SmolRci> FetchRcisBySessionAndBuilding(IEnumerable<string> sessions, IEnumerable<string> buildings);
        List<SmolRci> FetchRcisForRoom(string building, string room);

        List<FineSummary> FetchFinesByBuilding(IEnumerable<string> buildings);

        List<string> FetchBuildingCodes();
        List<ResidentHallGrouping> FetchBuildingMap();
        Dictionary<string, string> FetchBuildingCodeToBuildingNameMap();

        Room FetchRoom(string buildingCode, string roomNumber);

        List<RoomComponentType> FetchRoomComponentTypesForRci(int rciId);

        RoomAssignment FetchLatestRoomAssignmentForId(string id);
        List<RoomAssignment> FetchRoomAssignmentsThatDoNotHaveRcis(string buildingCode, string sessionCode);

        List<Session> FetchSessions();
        string FetchCurrentSession();
        
        Account FetchAccountByGordonId(string gordonId);
        List<Account> FetchResidentAccounts(string builingCode, string roomNumber, string sessionCode);

    }
}