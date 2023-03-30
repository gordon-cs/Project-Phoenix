using Moq;
using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Phoenix.UnitTests
{
    public class RciCheckoutUnitTests
    {
        // NOTE: This class was quickly made in order to debug a particular email error. 
        // IT is not a true unit tests because it does not assert anything. 
        // I have kept it in case this pops up again and we need to debug emailing.
        // If you do want to do so, don't forget to temporarily put in actual usernames/password combinations in the call to send a fine email method below

        private RciCheckoutService CheckoutService { get; set; }

        private Mock<IDatabaseDal> DatabaseDal { get; set; }

        private Mock<IDashboardService> DashboardService { get; set; }

        public RciCheckoutUnitTests()
        {
            this.DatabaseDal = new Mock<IDatabaseDal>();

            this.DashboardService = new Mock<IDashboardService>();

            this.CheckoutService = new RciCheckoutService(this.DatabaseDal.Object, this.DashboardService.Object);
        }

        [Fact]
        public void Test()
        {
            this.DatabaseDal.Setup(x => x.FetchRciById(It.IsAny<int>()))
                .Returns(new BigRci
                {
                    BuildingCode = "Test",
                    RoomNumber = "Test",
                    RoomOrApartmentResidents = new List<Account>(),
                    RoomComponentTypes = new List<RoomComponentType>
                    {
                        new RoomComponentType
                        {
                            BuildingCode = "Test",
                            RciTypeName = "Test",
                            RoomArea = "Test",
                            RoomComponentName = "Test",
                            RoomComponentTypeId = 1
                        }
                    },
                    Fines = new List<Fine>
                    {
                        new Fine
                        {
                            RoomComponentTypeId = 1,
                            Amount = 10,
                            GordonId = "401",
                            Reason = "Test"
                        }
                    },
                    Damages = new List<Damage>()

                });

            this.DatabaseDal.Setup(x => x.FetchAccountByGordonId(It.IsAny<string>()))
                .Returns(new Account
                {
                    FirstName = "TEST",
                    Email = "ENTER EMAIL HERE"
                });

            //Bennett Forkner - 03/30/2023 09:21 AM - Commenting out per request from Jeff Carpenter, CTS Ticket 162006
            //this.CheckoutService.SendFineEmail(1, "ENTER EMAIL HERE", "ENTER PASSWORD HERE");
        }
    }
}
