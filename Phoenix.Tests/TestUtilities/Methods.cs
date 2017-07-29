using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Phoenix.Tests.TestUtilities
{
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



    }
}
