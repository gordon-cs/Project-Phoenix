using Dapper;
using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;
using Phoenix.UnitTests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

       
        [Fact]
        public void FetchRciById_Succeeds()
        {
            // Arrange
            int existingRciId = 7065;

            // Test
            var result = this.Dal.FetchRciById(existingRciId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.RciId);
            Assert.NotNull(result.SessionCode);
            Assert.NotNull(result.RciComponents);
            Assert.NotEqual(0, result.RciComponents.Count);
            Assert.NotNull(result.RciComponents[0].Fines);
            Assert.NotNull(result.RciComponents[0].Damages);
            Assert.NotNull(result.CommonAreaSignatures);
        }

        [Fact]
        public void FetchRciById_Throws_When_No_Rci_Exists_For_The_RciId()
        {
            var exception = Assert.Throws<Exception>(() => this.Dal.FetchRciById(0));

            Assert.Contains("Expected a single result", exception.Message);
        }

        [Fact]
        public void FetchRciBySessionAndBuilding_Returns_Empty_List_When_No_Sessions_Or_Buildings_Are_Given()
        {
            var result = this.Dal.FetchRciBySessionAndBuilding(new List<string>(), new List<string>());

            Assert.Empty(result);
        }

        [Fact]
        public void FetchRciBySessionAndBuilding_Successeds()
        {
            var sessions = new List<string> { "201709" };
            var buildings = new List<string> { "TAV" };

            var result = this.Dal.FetchRciBySessionAndBuilding(sessions, buildings);

            Assert.NotEmpty(result);
            Assert.True(result.TrueForAll(x => x.SessionCode.Equals("201709")));
            Assert.True(result.TrueForAll(x => x.BuildingCode.Equals("TAV")));

            var rciIds = result.Select(x => x.RciId);
            var duplicateRciIds = rciIds.GroupBy(r => r).Any(x => x.Count() > 1);


            Assert.False(duplicateRciIds);
        }

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
        }

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
    }
}
