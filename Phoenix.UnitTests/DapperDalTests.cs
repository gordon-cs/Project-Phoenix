using Phoenix.DapperDal;
using Phoenix.UnitTests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Phoenix.UnitTests
{
    public class DapperDalTests : IClassFixture<DatabaseFixture>
    {
        private readonly IDal Dal;

        private IDbConnectionFactory DbConnectionFactory { get; set; }
        
        public DapperDalTests(DatabaseFixture fixture)
        {
            this.DbConnectionFactory = fixture.DbFactory;

            this.Dal = new DapperDal.DapperDal(this.DbConnectionFactory);

            SlapperAutoMapperInit.Initialize();
        }

        // Rcis
        [Fact]
        public void FetchRciById_Succeeds()
        {
            // Arrange
            var rciId = this.Dal.CreateNewDormRci("TESTID", "TAV", "104", "TESTSESSION");

            // Test
            var result = this.Dal.FetchRciById(rciId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.RciId);
            Assert.NotNull(result.SessionCode);
            Assert.NotNull(result.RoomComponentTypes);
            Assert.NotEqual(0, result.RoomComponentTypes.Count);
            Assert.NotNull(result.Fines);
            Assert.DoesNotContain(result.Fines, x => x.FineId == 0);
            Assert.NotNull(result.Damages);
            Assert.DoesNotContain(result.Damages, x => x.DamageId == 0);

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void FetchRciById_Throws_When_No_Rci_Exists_For_The_RciId()
        {
            var exception = Assert.Throws<Exception>(() => this.Dal.FetchRciById(0));

            Assert.Contains("Expected a single result", exception.Message);
        }

        [Fact]
        public void FetchRcisByBuilding_Success()
        {
            var buildings = new List<string> { "TAV" };

            var result = this.Dal.FetchRcisByBuilding(buildings);

            Assert.NotEmpty(result);
            Assert.True(result.TrueForAll(x => x.BuildingCode.Equals("TAV")));
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
            var rciId = this.Dal.CreateNewDormRci("4343", "TAV", "32", "201709");
            var sessions = new List<string> { "201709" };
            var buildings = new List<string> { "TAV" };

            var result = this.Dal.FetchRcisBySessionAndBuilding(sessions, buildings);

            Assert.NotEmpty(result);
            Assert.True(result.TrueForAll(x => x.SessionCode.Equals("201709")));
            Assert.True(result.TrueForAll(x => x.BuildingCode.Equals("TAV")));
        }

        [Fact]
        public void FetchRcisByRoom_Success()
        {
            // Arrange
            var buildingCode = "GRA";
            var roomNumber = "113";
            var rciId = this.Dal.CreateNewDormRci("TESTID", buildingCode, roomNumber, "TESTSESSION");

            // Test
            var result = this.Dal.FetchRcisForRoom(buildingCode, roomNumber);

            Assert.NotEmpty(result);
            Assert.True(result.TrueForAll(x => x.BuildingCode.Equals(buildingCode)));
            Assert.True(result.TrueForAll(x => x.RoomNumber.Equals(roomNumber)));

            // Cleanup
            this.Dal.DeleteRci(rciId);
        }

        [Fact]
        public void CreateAndDeleteTests()
        {
            // Create an Rci
            var rciId = this.Dal.CreateNewDormRci("0101", "Hello", "20", "2020");
            var commonAreaRciId = this.Dal.CreateNewCommonAreaRci("Common", "2", "32");

            var rci = this.Dal.FetchRciById(rciId);
            var commonAreaRci = this.Dal.FetchRciById(commonAreaRciId);

            Assert.NotEqual(0, rciId);
            Assert.Equal("0101", rci.GordonId);
            Assert.Equal("Hello", rci.BuildingCode);
            Assert.Equal("20", rci.RoomNumber);
            Assert.Equal("2020", rci.SessionCode);
            Assert.Equal(1, rci.RciTypeId);

            Assert.NotEqual(0, commonAreaRciId);
            Assert.Null(commonAreaRci.GordonId);
            Assert.Equal("Common", commonAreaRci.BuildingCode);
            Assert.Equal("2", commonAreaRci.RoomNumber);
            Assert.Equal("32", commonAreaRci.SessionCode);
            Assert.Equal(2, commonAreaRci.RciTypeId);

            // Delete it
            this.Dal.DeleteRci(rciId);
            this.Dal.DeleteRci(commonAreaRciId);

            var exception = Assert.Throws<Exception>(() => this.Dal.FetchRciById(rciId));
            var commonAreaException = Assert.Throws<Exception>(() => this.Dal.FetchRciById(commonAreaRciId));

            Assert.Contains("Expected a single result", exception.Message);
            Assert.Contains("Expected a single result", commonAreaException.Message);
        }

        [Fact]
        public void UpdateRciIsCurrentColumnTests()
        {
            // Create an rci
            var rciId1 = this.Dal.CreateNewDormRci("010101", "Test", "2020202", "2020202");
            var rciId2 = this.Dal.CreateNewDormRci("939393", "Test", "4434334", "3233232");
            var rciId3 = this.Dal.CreateNewDormRci("49494", "Test", "3444", "3232332");

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

        // Building Codes
        [Fact]
        public void FetchBuildingCodes_Success()
        {
            var results = this.Dal.FetchBuildingCodes();

            Assert.NotNull(results);
            Assert.NotEmpty(results);
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

        // Account
        [Fact]
        public void FetchAccount_Success()
        {
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

        // Room Assign
        [Fact]
        public void FetchLatestRoomAssign_Invalid_Id_Returns_Null()
        {
            var id = "43";

            var result = this.Dal.FetchLatestRoomAssignmentForId(id);

            Assert.Null(result);
        }

        [Fact]
        public void FetchLatestRoomAssign_Success()
        {
            var id = "50196746";

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
        public void FineSummary_Tests()
        {
            var noBuildings = new List<string>();

            var oneBuilding = new List<string> { "FUL" };

            var noBuildingResult = this.Dal.FetchFinesByBuilding(noBuildings);
            var oneBuildingResult = this.Dal.FetchFinesByBuilding(oneBuilding);

            Assert.Empty(noBuildingResult);

            Assert.NotEmpty(oneBuildingResult);

            var fineSummary = oneBuildingResult.First();
            Assert.NotNull(fineSummary.FineId);
            Assert.NotNull(fineSummary.GordonId);
            Assert.NotNull(fineSummary.LastName);
            Assert.NotNull(fineSummary.FirstName);
            Assert.NotNull(fineSummary.BuildingCode);
            Assert.NotNull(fineSummary.RoomNumber);
            Assert.NotNull(fineSummary.RoomComponentName);
            Assert.NotNull(fineSummary.SessionCode);
            Assert.NotNull(fineSummary.SuggestedCostsString);
            Assert.Contains(fineSummary.BuildingCode, oneBuilding);
        }

        // Rooms
        [Fact]
        public void FetchRoom_Success()
        {
            var room = this.Dal.FetchRoom("TAV", "104");

            Assert.Equal("TAV", room.BuildingCode);
            Assert.Equal("104", room.RoomNumber);
        }

        // Room Component Types
        [Fact]
        public void FetchRoomComponentTypesForRci_Success()
        {
            // Setup
            var rciId = this.Dal.CreateNewDormRci("40404", "TAV", "201", "230230");
            var commonAreaRciId = this.Dal.CreateNewCommonAreaRci("TAV", "201", "30202");

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
