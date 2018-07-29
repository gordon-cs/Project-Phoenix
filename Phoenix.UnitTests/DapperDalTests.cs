using Dapper;
using Phoenix.DapperDal;
using Phoenix.Exceptions;
using Phoenix.UnitTests.TestUtilities;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Phoenix.UnitTests
{
    public class DapperDalTests : IClassFixture<DatabaseFixture>
    {
        private readonly IDatabaseDal Dal;

        private IDbConnectionFactory DbConnectionFactory { get; set; }

        // Some handy values to use for testing.
        // Some of these values matter, meaning they need to be real values that exist in the database.
        // e.g. Session -> We do an inner join on it to fetch rcis, so it needs to be a valid session. Same for GordonId and BuildingCode
        // The other values could probably be made up, but we still choose to use "sane" values for them.
        private const string TestSession = "201809";

        private const string TestGordonId = "999999097";

        private const string TestGordonId2 = "999999098";

        private const string TestBuildingCode = "FER";

        private const string TestBuildingCode2 = "BRO";

        private const string TestRoomNumber = "109";

        private const string TestRoomNumber2 = "200";

        private const int TestRoomComponentTypeId = 1;

        public DapperDalTests(DatabaseFixture fixture)
        {
            this.DbConnectionFactory = fixture.DbFactory;

            this.Dal = new DapperDal.DapperDal(this.DbConnectionFactory);

            SlapperAutoMapperInit.Initialize();
        }

        // Rcis
        [Fact]
        public void FetchRcisComparisonTests()
        {
            // We want to make sure that we get the same number of rows back when we do a normal select statement vs when we qualify it with inner joins etc...
            string simpleSelectStatement = "select * from Rci";

            string selecteStatementToBeTested = DapperDal.Sql.Rci.RciSelectstatement;

            using (var connection = this.DbConnectionFactory.CreateConnection())
            {
                var first = connection.Query(simpleSelectStatement);

                var second = connection.Query(selecteStatementToBeTested);

                Assert.Equal(first.Count(), second.Count());
            }
        }

        [Fact]
        public void FetchRciById_Succeeds()
        {
            // Arrange
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            // Test
            var result = this.Dal.FetchRciById(rciId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.RciId);
            Assert.NotNull(result.SessionCode);
            Assert.NotNull(result.RoomComponentTypes);
            Assert.NotEmpty(result.RoomComponentTypes);
            Assert.NotNull(result.Fines);
            Assert.DoesNotContain(result.Fines, x => x.FineId == 0);
            Assert.NotNull(result.Damages);
            Assert.DoesNotContain(result.Damages, x => x.DamageId == 0);
            Assert.NotEmpty(result.RoomOrApartmentResidents);

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void FetchRciById_Throws_When_No_Rci_Exists_For_The_RciId()
        {
            var exception = Assert.Throws<RciNotFoundException>(() => this.Dal.FetchRciById(0));
        }

        [Fact]
        public void FetchRcisById_Success()
        {
            // Setup - Create 2 rcis
            var rciId1 = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);
            var rciId2 = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode2, TestRoomNumber2, TestSession);


            var rciList = this.Dal.FetchRcisById(new List<int> { rciId1, rciId2 });

            var rciIdlist = rciList.Select(x => x.RciId);

            Assert.Equal(2, rciList.Count);
            Assert.Contains(rciId1, rciIdlist);
            Assert.Contains(rciId2, rciIdlist);

            // Cleanup
            foreach(var rciId in rciIdlist)
            {
                this.Dal.DeleteRci(rciId);
            }
        }

        [Fact]
        public void FetchRcisByBuilding_Success()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            var buildings = new List<string> { TestBuildingCode };

            var result = this.Dal.FetchRcisByBuilding(buildings);

            Assert.NotEmpty(result);

            Assert.Contains(result, x => x.BuildingCode.Equals(TestBuildingCode) && x.RoomNumber.Equals(TestRoomNumber) && x.GordonId.Equals(TestGordonId));
        }

        [Fact]
        public void FetchRcisBySessionAndBuilding_Returns_Empty_List_When_No_Sessions_Or_Buildings_Are_Given()
        {
            var result = this.Dal.FetchRcisBySessionAndBuilding(new List<string>(), new List<string>());

            Assert.Empty(result);
        }

        [Fact]
        public void FetchRcisBySessionAndBuilding_Success()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var sessions = new List<string> { TestSession };
            var buildings = new List<string> { TestBuildingCode };

            // Test
            var result = this.Dal.FetchRcisBySessionAndBuilding(sessions, buildings);

            // Assert
            Assert.NotEmpty(result);

            Assert.Contains(result, x => x.BuildingCode.Equals(TestBuildingCode) && x.RoomNumber.Equals(TestRoomNumber) && x.GordonId.Equals(TestGordonId));

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void FetchRcisByRoom_Success()
        {
            // Arrange
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            // Test
            var result = this.Dal.FetchRcisForRoom(TestBuildingCode, TestRoomNumber);

            Assert.NotEmpty(result);

            Assert.Contains(result, x => x.BuildingCode.Equals(TestBuildingCode) && x.RoomNumber.Equals(TestRoomNumber) && x.GordonId.Equals(TestGordonId));

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void CreateAndDeleteTests()
        {
            // Create an Rci
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var commonAreaRciId = this.Dal.CreateNewCommonAreaRci(TestBuildingCode2, TestRoomNumber2, TestSession);

            var rci = this.Dal.FetchRciById(rciId);
            var commonAreaRci = this.Dal.FetchRciById(commonAreaRciId);

            Assert.NotEqual(0, rciId);
            Assert.Equal(TestGordonId, rci.GordonId);
            Assert.Equal(TestBuildingCode, rci.BuildingCode);
            Assert.Equal(TestRoomNumber, rci.RoomNumber);
            Assert.Equal(TestSession, rci.SessionCode);
            Assert.Equal(1, rci.RciTypeId);

            Assert.NotEqual(0, commonAreaRciId);
            Assert.Null(commonAreaRci.GordonId);
            Assert.Equal(TestBuildingCode2, commonAreaRci.BuildingCode);
            Assert.Equal(TestRoomNumber2, commonAreaRci.RoomNumber);
            Assert.Equal(TestSession, commonAreaRci.SessionCode);
            Assert.Equal(2, commonAreaRci.RciTypeId);

            // Delete it
            this.Dal.DeleteRci(rciId);
            this.Dal.DeleteRci(commonAreaRciId);

            var exception = Assert.Throws<RciNotFoundException>(() => this.Dal.FetchRciById(rciId));
            var commonAreaException = Assert.Throws<RciNotFoundException>(() => this.Dal.FetchRciById(commonAreaRciId));
        }

        [Fact]
        public void RciCascadeDeleteTests()
        {
            // We want to test the DB cascade delete functionality.
            // If everything goes well, we should be able to create an rci,
            // and create a bunch of fines and damages using that rciId.
            // If that rci is deleted, all the fines and damages we created should also be deleted.

            var rciId = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);

            var fineId = this.Dal.CreateNewFine(10, TestGordonId, "NO", rciId, TestRoomComponentTypeId);
            var damageId = this.Dal.CreateNewDamage("NO", null, rciId, TestGordonId, TestRoomComponentTypeId);

            // First check to make sure we can fetch both the fine and damage
            var fine = this.Dal.FetchFineById(fineId);
            var damage = this.Dal.FetchDamageById(damageId);

            Assert.NotNull(fine);
            Assert.NotNull(damage);

            // Now delete the rci
            this.Dal.DeleteRci(rciId);

            // Now try to re-fetch our fine and damage, we should get exceptions
            var fineException = Assert.Throws<FineNotFoundException>(() => this.Dal.FetchFineById(fineId));
            var damageException = Assert.Throws<DamageNotFoundException>(() => this.Dal.FetchDamageById(damageId));
        }

        [Fact]
        public void UpdateRciIsCurrentColumnTests()
        {
            // Create an rci
            var rciId1 = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var rciId2 = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var rciId3 = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);

            // Now Fetch the rcis
            var rci1 = this.Dal.FetchRciById(rciId1);
            var rci2 = this.Dal.FetchRciById(rciId2);
            var rci3 = this.Dal.FetchRciById(rciId3);

            Assert.True(rci1.IsCurrent);
            Assert.True(rci2.IsCurrent);
            Assert.True(rci2.IsCurrent);

            this.Dal.SetRciIsCurrentColumn(new List<int> { rciId1, rciId2, rciId3 }, false);

            rci1 = this.Dal.FetchRciById(rciId1);
            rci2 = this.Dal.FetchRciById(rciId2);
            rci3 = this.Dal.FetchRciById(rciId3);

            Assert.False(rci1.IsCurrent);
            Assert.False(rci2.IsCurrent);
            Assert.False(rci2.IsCurrent);

            this.Dal.SetRciIsCurrentColumn(new List<int> { rciId1, rciId2, rciId3 }, true);

            rci1 = this.Dal.FetchRciById(rciId1);
            rci2 = this.Dal.FetchRciById(rciId2);
            rci3 = this.Dal.FetchRciById(rciId3);

            Assert.True(rci1.IsCurrent);
            Assert.True(rci2.IsCurrent);
            Assert.True(rci2.IsCurrent);

            // Cleanup
            this.Dal.DeleteRci(rciId1);
            this.Dal.DeleteRci(rciId2);
            this.Dal.DeleteRci(rciId3);
        }

        [Fact]
        public void UpdateRciGordonIdColumnTests()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            var rci = this.Dal.FetchRciById(rciId);

            Assert.Equal(TestGordonId, rci.GordonId);

            // Test
            this.Dal.SetRciGordonIdColumn(new List<int> { rciId }, TestGordonId2);

            // Assert
            Assert.Equal(TestGordonId2, this.Dal.FetchRciById(rciId).GordonId);

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void UpdateRciCheckinDateAndGordonIdTests()
        {
            var today = DateTime.Now;

            // Setup
            var rciId1 = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);
            var rciId2 = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            // Test 
            this.Dal.SetRciCheckinDateColumns(new List<int> { rciId1, rciId2 }, today, today, today, today);
            this.Dal.SetRciCheckinGordonIdColumns(new List<int> { rciId1, rciId2 }, "hello", "hello");

            // Assert
            var rci1 = this.Dal.FetchRciById(rciId1);
            var rci2 = this.Dal.FetchRciById(rciId2);

            Assert.NotNull(rci1.ResidentCheckinDate);
            Assert.NotNull(rci1.RaCheckinDate);
            Assert.NotNull(rci1.RdCheckinDate);
            Assert.NotNull(rci1.CheckinRaGordonId);
            Assert.NotNull(rci1.CheckinRdGordonId);

            Assert.NotNull(rci2.ResidentCheckinDate);
            Assert.NotNull(rci2.RaCheckinDate);
            Assert.NotNull(rci2.RdCheckinDate);
            Assert.NotNull(rci2.CheckinRaGordonId);
            Assert.NotNull(rci2.CheckinRdGordonId);

            Assert.NotNull(rci1.LifeAndConductSignatureDate);
            Assert.NotNull(rci2.LifeAndConductSignatureDate);

            // Cleanup
            this.Dal.DeleteRci(rciId1);
            this.Dal.DeleteRci(rciId2);
        }

        [Fact]
        public void UpdateRciCheckoutDateAndGordonIdTests()
        {
            var today = DateTime.Now;

            // Setup
            var rciId1 = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);
            var rciId2 = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            //Test 
            this.Dal.SetRciCheckoutDateColumns(new List<int> { rciId1, rciId2 }, today, today, today);
            this.Dal.SetRciCheckoutGordonIdColumns(new List<int> { rciId1, rciId2 }, "hello", "hello2");

            // Assert
            var rci1 = this.Dal.FetchRciById(rciId1);
            var rci2 = this.Dal.FetchRciById(rciId2);

            Assert.Equal("hello", rci1.CheckoutRaGordonId);
            Assert.Equal("hello", rci2.CheckoutRaGordonId);
            Assert.Equal("hello2", rci1.CheckoutRdGordonId);
            Assert.Equal("hello2", rci2.CheckoutRdGordonId);

            Assert.NotNull(rci1.ResidentCheckoutDate);
            Assert.NotNull(rci1.RaCheckoutDate);
            Assert.NotNull(rci1.RdCheckoutDate);

            Assert.NotNull(rci2.ResidentCheckoutDate);
            Assert.NotNull(rci2.RaCheckoutDate);
            Assert.NotNull(rci2.RdCheckoutDate);

            // Cleanup
            this.Dal.DeleteRci(rciId1);
            this.Dal.DeleteRci(rciId2);
        }

        // Damages
        [Fact]
        public void CreateUpdateAndDeleteDamageTests()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            // Create a new damage
            var damageId = this.Dal.CreateNewDamage("Tack on the wall", null, rciId, "50153295", 2);

            Assert.NotEqual(0, damageId);

            // Update said damage
            this.Dal.UpdateDamage(damageId, null, "test", null, null);

            var damage = this.Dal.FetchDamageById(damageId);

            Assert.Equal("Tack on the wall", damage.Description);
            Assert.Equal("test", damage.ImagePath);
            Assert.Equal(rciId, damage.RciId);
            Assert.Equal("50153295", damage.GordonId);
            Assert.Equal(2, damage.RoomComponentTypeId);

            // Cleanup
            this.Dal.DeleteDamage(damageId); // Also serves as a test of the damage delete functionality.
            this.Dal.DeleteRci(rciId);
        }

        // Common Are Rci Signatures
        [Fact]
        public void CreateAndDeleteCommonAreaRciTests()
        {
            // Setup
            var rciId = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);

            var commonAreaRciSignature = this.Dal.CreateNewCommonAreaRciSignature("50153295", rciId, DateTime.Now, Constants.CHECKIN);

            Assert.NotNull(commonAreaRciSignature);
            Assert.Equal(rciId, commonAreaRciSignature.RciId);
            Assert.Equal(Constants.CHECKIN, commonAreaRciSignature.SignatureType);
            Assert.Equal("50153295", commonAreaRciSignature.GordonId);

            // Cleanup
            this.Dal.DeleteCommonAreaRciSignature("50153295", rciId, Constants.CHECKIN);
            this.Dal.DeleteRci(rciId);
        }

        // Building Codes
        [Fact]
        public void FetchBuildingCodes_Success()
        {
            var results = this.Dal.FetchBuildingCodes();

            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void FetchBuildingCoddeToBuildingDescription_Success()
        {
            var result = this.Dal.FetchBuildingCodeToBuildingNameMap();

            Assert.NotNull(result);

            Assert.Equal("Rider Hall", result["RID"]);
        }

        [Fact]
        public void FetchBuildingMap_Success()
        {
            var results = this.Dal.FetchBuildingMap();

            results.ForEach(x => Assert.NotEmpty(x.BuildingCodes));
        }

        // Sessions
        [Fact]
        public void FetchSessions_Success()
        {
            var results = this.Dal.FetchSessions();

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.NotNull(results[0].SessionCode);
            Assert.NotNull(results[0].SessionDescription);
            Assert.NotNull(results[0].SessionStartDate);
            Assert.NotNull(results[0].SessionEndDate);
        }

        [Fact]
        public void FetchCurrentSession_Success()
        {
            // We just want to make sure the value is not null
            var result = this.Dal.FetchCurrentSession();

            Assert.NotNull(result);
        }

        // Account
        [Fact]
        public void FetchAccount_Success()
        {
            // The following values need to be chosen carefullly.
            // They need to be ids of actual people in the Accounts table with the right permissions. Meaning one needs to be a student,
            // one needs to be an ra and the third an rd.

            // Tavilla Resident
            var studentId = "50196746";

            // Drew Ra
            var raId = "50159101";

            // Road Hall RD
            var rdId = "9529423";

            var studentResult = this.Dal.FetchAccountByGordonId(studentId);

            var raResult = this.Dal.FetchAccountByGordonId(raId);

            var rdResult = this.Dal.FetchAccountByGordonId(rdId);

            Assert.NotNull(studentResult);
            Assert.NotNull(raResult);
            Assert.NotNull(rdResult);

            Assert.NotNull(studentResult.GordonId);
            Assert.False(studentResult.IsRa);
            Assert.False(studentResult.IsRd);
            Assert.False(studentResult.IsAdmin);
            Assert.Null(studentResult.RaBuildingCode);
            Assert.Null(studentResult.RdHallGroup);

            Assert.NotNull(raResult.GordonId);
            Assert.True(raResult.IsRa);
            Assert.False(raResult.IsRd);
            Assert.False(raResult.IsAdmin);
            Assert.NotNull(raResult.RaBuildingCode);
            Assert.Null(raResult.RdHallGroup);

            Assert.NotNull(rdResult.GordonId);
            Assert.False(rdResult.IsRa);
            Assert.True(rdResult.IsRd);
            Assert.False(rdResult.IsAdmin);
            Assert.Null(rdResult.RaBuildingCode);
            Assert.NotNull(rdResult.RdHallGroup);
        }

        [Fact]
        public void FetchResidentAccounts()
        {
            // These values were intentionally chosen. If you change them, make sure you change the assert statements accordingly
            var buildingCode = "BRO";
            var apartmentNumber = "203";
            var session = "201509";

            // The fields will be all null (except for the id) because these are alumni residents
            var results = this.Dal.FetchResidentAccounts(buildingCode, apartmentNumber, session);

            Assert.NotEmpty(results);
            Assert.Equal(4, results.Count);
        }

        // Room Assign
        [Fact]
        public void FetchLatestRoomAssign_Invalid_Id_Throws()
        {
            var id = "43";

            Assert.Throws<RoomAssignNotFoundException>(() => this.Dal.FetchLatestRoomAssignmentForId(id));
        }

        [Fact]
        public void FetchLatestRoomAssign_Success()
        {
            var id = TestGordonId;

            var result = this.Dal.FetchLatestRoomAssignmentForId(id);

            Assert.NotNull(result);
            Assert.NotNull(result.GordonId);
            Assert.NotNull(result.BuildingCode);
            Assert.NotNull(result.RoomNumber);
            Assert.NotNull(result.AssignmentDate);
            Assert.NotNull(result.SessionCode);
        }

        [Fact]
        public void FetchRoomAssignmentsThatDoNotHaveRcis_Success()
        {
            // Use an old SessionCode so that we won't find matching rcis
            var sessionCode = "201109";
            var building = "TAV";

            var result = this.Dal.FetchRoomAssignmentsThatDoNotHaveRcis(building, sessionCode);

            // 150 is the number of RoomAssignments that were made in the sessionCode "201109"
            // Well technically, it's 151, but that's because of a record with ID_NUM of null, which our method ignores.
            Assert.Equal(150, result.Count);

        }

        // Fines
        [Fact]
        public void CreateUpdateAndDeleteFine_Tests()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);

            // Create a new fine
            var fineId = this.Dal.CreateNewFine(10, "50153295", "No reason", rciId, TestRoomComponentTypeId);

            Assert.NotEqual(0, fineId);

            // Update said damage
            this.Dal.UpdateFine(fineId, null, null, "A reason", null, 3);

            var fine = this.Dal.FetchFineById(fineId);

            Assert.Equal("A reason", fine.Reason);
            Assert.Equal("50153295", fine.GordonId);
            Assert.Equal(rciId, fine.RciId);
            Assert.Equal(10, fine.Amount);
            Assert.Equal(3, fine.RoomComponentTypeId);

            // Cleanup
            this.Dal.DeleteFine(fineId); // Also serves as a test of the fine delete functionality.
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void FineSummary_Tests()
        {
            // Setup
            var noBuildings = new List<string>();
            var oneBuilding = new List<string> { TestBuildingCode };

            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var fineId = this.Dal.CreateNewFine(10, TestGordonId, "NO REASON", rciId, TestRoomComponentTypeId);


            var noBuildingResult = this.Dal.FetchFinesByBuilding(noBuildings);
            var oneBuildingResult = this.Dal.FetchFinesByBuilding(oneBuilding);

            Assert.Empty(noBuildingResult);
            Assert.NotEmpty(oneBuildingResult);

            var fineSummary = oneBuildingResult.Where(x => x.FineId.Equals(fineId)).First();
            Assert.Equal(TestGordonId, fineSummary.GordonId);
            Assert.NotNull(fineSummary.LastName);
            Assert.NotNull(fineSummary.FirstName);
            Assert.Equal(TestBuildingCode, fineSummary.BuildingCode);
            Assert.Equal(TestRoomNumber, fineSummary.RoomNumber);
            Assert.NotNull(fineSummary.RoomComponentName);
            Assert.Equal(TestSession, fineSummary.SessionCode);
            Assert.NotNull(fineSummary.SuggestedCostsString);

            // Cleanup
            this.Dal.DeleteFine(fineId);
            this.Dal.DeleteRci(rciId);
        }

        // Rooms
        [Fact]
        public void FetchRoom_Success()
        {
            var room = this.Dal.FetchRoom(TestBuildingCode, TestRoomNumber);

            Assert.Equal(TestBuildingCode, room.BuildingCode);
            Assert.Equal(TestRoomNumber, room.RoomNumber);
        }

        // Room Component Types
        [Fact]
        public void FetchRoomComponentTypesForRci_Success()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci(TestGordonId, TestBuildingCode, TestRoomNumber, TestSession);
            var commonAreaRciId = this.Dal.CreateNewCommonAreaRci(TestBuildingCode, TestRoomNumber, TestSession);

            // Test
            var roomComponentTypesDorm = this.Dal.FetchRoomComponentTypesForRci(rciId);
            var roomComponentTypesCommon = this.Dal.FetchRoomComponentTypesForRci(commonAreaRciId);

            // Assert
            Assert.NotEmpty(roomComponentTypesDorm);
            Assert.NotEmpty(roomComponentTypesCommon);
            Assert.True(roomComponentTypesDorm.TrueForAll(x => x.RciTypeName.Contains("Dorm")));
            Assert.True(roomComponentTypesCommon.TrueForAll(x => x.RciTypeName.Contains("Common")));

            // Cleanup 
            this.Dal.DeleteRci(rciId);
            this.Dal.DeleteRci(commonAreaRciId);
        }
    }
}
