using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Phoenix.Tests.Models;
using Phoenix.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Tests.Pages
{
    public class DashboardPage
    {
        public IWebDriver webDriver;

        private static By menuButton = By.ClassName("dropdown");
        private static By logoutButton = By.ClassName("logout-button");
        private static By legend = By.ClassName("legends-wrapper");
        private static By rciCards = By.ClassName("rci-card");
        private static By rciLinks = By.CssSelector("[data-selenium-id='dashboard-page-rci-button']");
        private static By rciCardBuildingAndRoom = By.CssSelector("[data-selenium-id='dashboard-page-rci-building-and-room']");
        private static By rciCardStudentName = By.CssSelector("[data-selenium-id='dashboard-page-rci-student-name']");
        private static By rciCardCheckin = By.ClassName("rci-card-checkin");
        private static By rciCardCheckout = By.ClassName("rci-card-checkout");
        private static By rciSignatureBlocks = By.CssSelector("[data-selenium-id='dashboard-page-signature-blocks']");

        public DashboardPage(IWebDriver driver)
        {
            webDriver = driver;

            // Wait for the legend to be visible.
            try
            {
                Methods.WaitForElementToBeVisible(driver, legend);
            }
            catch (TimeoutException e)
            {
                // We are not on the right page
                throw new IllegalStateException(string.Format("This is not the Dashboard page. Current page title: {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }
        }

        /// <summary>
        /// Finds the logout button and clicks on it.
        /// </summary>
        /// <returns>A LoginPage object</returns>
        public LoginPage Logout()
        {
            webDriver.FindElement(menuButton).Click();
            try
            {
                new WebDriverWait(webDriver, Values.ONE_MOMENT).Until(ExpectedConditions.ElementToBeClickable(logoutButton));
            }
            catch (TimeoutException e)
            {
                throw new TimeoutException("Could not find the logout button.", e);
            }
            
            webDriver.FindElement(logoutButton).Click();

            return new LoginPage(webDriver);
        }

        /// <summary>
        /// Select and click on a random Rci on the dashboard
        /// </summary>
        /// <returns>A GeneralRciPage object</returns>
        public GenericRciPage SelectRandomRci()
        {
            var rciList = webDriver.FindElements(rciCards);

            rciList[Methods.GetRandomInteger(0, rciList.Count)].FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Select and click on a random Rci from that dashboard that is for a specific hall
        /// </summary>
        /// <param name="buildingCode">The building code to look for</param>
        /// <returns>A GeneralRcipage object</returns>
        public GenericRciPage SelectRandomRciForBuilding(string buildingCode)
        {
            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardBuildingAndRoom).Text.Contains(buildingCode));

            rciList.ElementAt(Methods.GetRandomInteger(0, rciList.Count())).FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Select and click on the first rci that has the given name on it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GenericRciPage SelectFirstRciWithName(string name)
        {
            name = name.ToLower().Trim();

            var rciList = webDriver.FindElements(rciCards);

            var rci = rciList.Where(m => m.FindElement(rciCardStudentName).Text.ToLower().Contains(name)).FirstOrDefault();

            if (rci == null)
            {
                throw new NoSuchElementException(string.Format("Rci card with name {0} was not found", name));
            }

            rci.FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Select and click on a random rci that is in the checkin state.
        /// </summary>
        /// <param name="signedByResident">The rci is signed by the resident</param>
        /// <param name="signedByRA">The rci is signed by the RA</param>
        /// <returns>A GeneralRciPage object</returns>
        public GenericRciPage SelectRandomCheckinRci(bool signedByResident, bool signedByRA)
        {
            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardCheckin) != null);

            if (!signedByResident && !signedByRA) // unsigned
            {
                // No signature blocks
                rciList = rciList.Where(m => m.FindElement(rciSignatureBlocks).FindElements(By.TagName("span")).Count == 0);
            }
            else if (signedByResident && !signedByRA) // signed by Resident only
            {
                // Only an RA signature block
                rciList = rciList.Where(m => 
                    m.FindElement(rciSignatureBlocks).Text.Contains("RES") && 
                    !m.FindElement(rciSignatureBlocks).Text.Contains("RA"));
            }
            else if (signedByResident && signedByRA) // signed by Resident and RA
            {
                rciList = rciList.Where(m => 
                    m.FindElement(rciSignatureBlocks).Text.Contains("RES") && 
                    m.FindElement(rciSignatureBlocks).Text.Contains("RA"));
            }
            else // undefined state
            {
                throw new IllegalStateException("No Rci can be found in this state.");
            }

            rciList.ElementAt(Methods.GetRandomInteger(0, rciList.Count())).FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);

        }

        /// <summary>
        /// Select a random rci that is in the checkout state
        /// </summary>
        /// <param name="signedByResident">The rci is signed by the resident</param>
        /// <param name="signedByRA">The rci is signed by the RA</param>
        /// <param name="signedByRD">The rci is signed by the RD</param>
        /// <returns>A GeneralRciPage object</returns>
        public GenericRciPage SelectRandomCheckoutRci(bool signedByResident, bool signedByRA, bool signedByRD)
        {
            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardCheckout) != null);

            if (!signedByResident && !signedByRA && !signedByRD) // unsigned
            {
                // No signature blocks
                rciList = rciList.Where(m => m.FindElement(rciSignatureBlocks).FindElements(By.TagName("span")).Count == 0);
            }
            else if (signedByResident && !signedByRA && !signedByRD) // signed by Resident only
            {
                // Only an RA signature block
                rciList = rciList.Where(m =>
                    m.FindElement(rciSignatureBlocks).Text.Contains("RES") && 
                    !m.FindElement(rciSignatureBlocks).Text.Contains("RA") &&
                    !m.FindElement(rciSignatureBlocks).Text.Contains("RD"));
            }
            else if (signedByResident && signedByRA && !signedByRD) // signed by Resident and RA only
            {
                rciList = rciList.Where(m =>
                    m.FindElement(rciSignatureBlocks).Text.Contains("RES") &&
                    m.FindElement(rciSignatureBlocks).Text.Contains("RA") && 
                    !m.FindElement(rciSignatureBlocks).Text.Contains("RD"));
            }
            else if(signedByResident && signedByRA && signedByRD) // signed by Resident and RA and RD 
            {
                rciList = rciList.Where(m =>
                    m.FindElement(rciSignatureBlocks).Text.Contains("RES") &&
                    m.FindElement(rciSignatureBlocks).Text.Contains("RA") &&
                    m.FindElement(rciSignatureBlocks).Text.Contains("RD"));
            }
            else // undefined state
            {
                throw new IllegalStateException("No Rci can be found in this state.");
            }

            rciList.ElementAt(Methods.GetRandomInteger(0, rciList.Count())).FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Select and click on the rci with the given ID
        /// </summary>
        /// <param name="rciID">The rci ID</param>
        /// <returns>A GeneralRciPage object. Throws </returns>
        /// <exception cref="NotFoundException">If an rci with the given id was not found</exception>
        public GenericRciPage SelectRci(int rciID)
        {
            var expectedHrefAttribute = Values.START_URL + "/Dashboard/GotoRci?rciID=" + rciID;
            var rciList = webDriver.FindElements(rciLinks).Where(m => m.GetAttribute("href").Equals(expectedHrefAttribute));
            if (rciList.Count() > 1)
            {
                throw new IllegalStateException("There were multiple rci cards with the same href link.");
            }
            if (rciList.Count() < 1)
            {
                throw new NotFoundException("There was no rci card with the given href. Href: " + expectedHrefAttribute);
            }
            rciList.First().Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Select and click on the common area rci that matches the given parameters.
        /// </summary>
        /// <param name="apartmentNumber">The apartment number</param>
        /// <returns>A GeneralRciPage object</returns>
        public GenericRciPage SelectCommonAreaRci(string apartmentNumber)
        {
            string roomID = apartmentNumber.Trim().ToLower();

            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardBuildingAndRoom).Text.ToLower().Equals(roomID));

            // There should be only one entry
            if (rciList.Count() > 1)
            {
                throw new IllegalStateException("There was more than one common area rci for room " + roomID);
            }
            if (rciList.Count() < 1)
            {
                throw new NotFoundException("Could not find a common area rci for room " + roomID);
            }

            var commonAreaRci = rciList.First();
            commonAreaRci.FindElement(By.TagName("a")).Click();

            return new GenericRciPage(webDriver);
        }

        /// <summary>
        /// Get a list of all rci cards on the dashboard
        /// </summary>
        /// <returns>A list of RciCard objects</returns>
        public IEnumerable<RciCard> GetAllRciCards()
        {
            var webElementList = webDriver.FindElements(rciCards);
            var rciCardList = Enumerable.Empty<RciCard>();

            foreach(var element in webElementList)
            {
                rciCardList = rciCardList.Concat(new[] { new RciCard(element, webDriver) });
            }

            return rciCardList;
        }
        /// <summary>
        /// Get an RciCard object that represents the rci card on the dashboard
        /// </summary>
        /// <param name="rciID">The id of the rci we are looking for</param>
        /// <returns>An RciCard object.</returns>
        /// <exception cref="NotFoundException">If an rci card was not found with the given id </exception>
        /// <exception cref="IllegalStateException">If for some reason there were multiple rcis with the same id</exception>
        public RciCard GetRciCard(int rciID)
        {
            var expectedHrefAttribute = Values.START_URL + "/Dashboard/GotoRci?rciID=" + rciID;
            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciLinks).GetAttribute("href").Equals(expectedHrefAttribute));
            if (rciList.Count() > 1)
            {
                throw new IllegalStateException("There were multiple rci cards with the same href link.");
            }
            if (rciList.Count() < 1)
            {
                throw new NotFoundException("There was no rci card with the given href. Href: " + expectedHrefAttribute);
            }
            return new RciCard(rciList.First(), webDriver);
        }

        /// <summary>
        /// Get an RciCard object that represents the rci card on the dashboard with the given name
        /// </summary>
        /// <param name="name">The name of the owner of the rci we are looking for</param>
        /// <returns>An RciCard object.</returns>
        /// <exception cref="NotFoundException">If an rci card was not found with the given id </exception>
        /// <exception cref="IllegalStateException">If for some reason there were multiple rcis with the same id</exception>
        public RciCard GetRciCardWithName(string name)
        {
            name = name.ToLower().Trim();

            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardStudentName).Text.ToLower().Contains(name));

            if (rciList.Count() > 1)
            {
                throw new IllegalStateException("There were multiple rci cards with the same name on them.");
            }
            if (rciList.Count() < 1)
            {
                throw new NotFoundException("There was no rci card with the given name. Name: " + name);
            }
            return new RciCard(rciList.First(), webDriver);
        }

        /// <summary>
        /// Returns an RciCard object that represents the rci card for the apartment with the given number
        /// </summary>
        /// <param name="apartmentNumber">The apartment number</param>
        /// <returns>An RciCard object</returns>
        public RciCard GetCommonAreaRciCard(string apartmentNumber)
        {
            string roomID = apartmentNumber.Trim().ToLower();

            var rciList = webDriver.FindElements(rciCards).Where(m => m.FindElement(rciCardBuildingAndRoom).Text.ToLower().Equals(roomID));

            // There should be only one entry
            if (rciList.Count() > 1)
            {
                throw new IllegalStateException("There was more than one common area rci for room " + roomID);
            }
            if (rciList.Count() < 1)
            {
                throw new NotFoundException("Could not find a common area rci for room " +roomID);
            }

            return new RciCard(rciList.First(), webDriver);
        }

        /// <summary>
        /// The number of rcis on the dashboard
        /// </summary>
        /// <returns>The number of rcis on the dashboard</returns>
        public int RciCount()
        {
            return webDriver.FindElements(rciCards).Count;
        }


    }
}
