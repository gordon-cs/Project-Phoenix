using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Phoenix.Models;
using Phoenix.Tests.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Tests.TestUtilities
{
    /// <summary>
    /// A collection of often used methods
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Wait until the specified element is visible.
        /// </summary>
        /// <param name="driver">The web driver</param>
        /// <param name="locator">The element we are waiting for.</param>
        public static void WaitForElementToBeVisible(IWebDriver driver, By locator)
        {
            new WebDriverWait(driver, Values.DEFAULT_WAIT).Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Compares the function's parameter with the driver's Title attribute 
        /// </summary>
        /// <param name="title">The title of the page we are expecting</param>
        /// <param name="driver">The WebDriver</param>
        /// <returns>True if we are on the page with the title indicated. False if not.</returns>
        public static bool VerifyWeAreOnPageWithTitle(IWebDriver driver, string title)
        {
            return driver.Title.Equals(title);
        }

        /// <summary>
        /// Generate a random number between two bounds.
        /// </summary>
        /// <param name="min">The lower bound inclusive</param>
        /// <param name="max">The upper bound exclusive</param>
        /// <returns>A generated random numer</returns>
        public static int GetRandomInteger(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }

        /// <summary>
        /// Helper method to get the name of a person given their ID number
        /// The returned name is put in a dictionary
        /// </summary>
        /// <param name="id">The gordon id of the person we are trying to find.</param>
        /// <returns>A dictionary with entries for first and last name</returns>
        public static Dictionary<string,string> GetFullName(string id)
        {
            Dictionary<string, string> fullName = new Dictionary<string, string>() ;

            using (var db = new RCIContext())
            {
                Account acct;
                try
                {
                    acct = db.Account.Where(m => m.ID_NUM.Equals(id)).First();
                }
                catch(InvalidOperationException e)
                {
                    // Gets thrown if the sequence is empty
                    throw new NotFoundException("Could not find the account with id " + id, e);
                }

                fullName.Add("firstname", acct.firstname);
                fullName.Add("lastname", acct.lastname);
            }

            return fullName;
        }
        
        /// <summary>
        /// Given the gordon id of a person, find out their room id.
        /// Room ID = Building Code + room number
        /// The Room ID uniquely identifies every room on campus
        /// </summary>
        /// <param name="id">The gordon ID of the person</param>
        /// <returns>A dictionary with entries for building code and room number</returns>
        public static Dictionary<string, string> GetRoomID(string id)
        {
            Dictionary<string, string> roomID = new Dictionary<string, string>();

            using (var db = new RCIContext())
            {
                RoomAssign roomAssignRecord;
                try
                {
                    roomAssignRecord = db.RoomAssign.Where(m => m.ID_NUM.Value.ToString().Equals(id)).OrderByDescending(m => m.ASSIGN_DTE).First();
                }
                catch(InvalidOperationException e)
                {
                    // Gets thrown if the sequence is empty
                    throw new NotFoundException("Could not find a room assignment with id " + id, e);
                }

                var roomNumber = roomAssignRecord.ROOM_CDE;
                var buildingCode = roomAssignRecord.BLDG_CDE;
                roomID.Add("building", buildingCode);
                roomID.Add("room", roomNumber);
            }

            return roomID;
        }

    }
}
