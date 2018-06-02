using OpenQA.Selenium;
using Phoenix.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.Pages
{
    public class BatchSignaturePage
    {
        public IWebDriver webDriver;

        private static By signatureInput = By.CssSelector("[data-selenium-id='rci-sig']");
        private static By submitButton = By.CssSelector("[data-selenium-id='submit-button']");
        private static By checkedInputBox = By.CssSelector("[data-selenium-id='checked-input-box']");

        public BatchSignaturePage(IWebDriver driver)
        {
            webDriver = driver;

            var isBatchSignaturePage = Methods.VerifyWeAreOnPageWithTitle(webDriver, Values.BATCH_SIGNATURE_TITLE);

            if (!isBatchSignaturePage)
            {
                throw new IllegalStateException(string.Format("We are not on the Batch Signature page! Current page title {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }
        }

        /// <summary>
        /// Fill the input box with the provided signature
        /// </summary>
        /// <param name="signature">The text with which to fill the input box</param>
        /// <exception cref="IllegalStateException">If not signature input boxes exist</exception>
        public BatchSignaturePage Sign(string signature)
        {

            var inputBoxes = webDriver.FindElements(signatureInput);

            // When we call Sign(), we expect that there will signature boxes. 
            // If none exist, this means that the current user doesn't have the right to sign said rci.
            if (inputBoxes.Count() < 1)
            {
                throw new IllegalStateException("Expected to find exactly one input boxe, but none was found");
            }

            if (inputBoxes.Count() > 1)
            {
                throw new IllegalStateException($"Expected to find exactly one input box, but {inputBoxes.Count()} were found");
            }

            inputBoxes.Single().SendKeys(signature);

            return this;
        }

        /// <summary>
        /// Submit the signature by clicking the submit button
        /// </summary>
        /// <returns>A new DashboardPage object</returns>
        /// <exception cref="IllegalStateException">The button to submit signatures could not be found.</exception>
        public DashboardPage SubmitSignature()
        {
            try
            {
                webDriver.FindElement(submitButton).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new NoSuchElementException("Could not click on the button to submit the signature(s)", e);
            }

            return new DashboardPage(webDriver);
        }

        /// <summary>
        /// Figure out how many Rcis there are to sign by counting the number of checkeed input boxes on the page.
        /// </summary>
        /// <returns>The Number of Rcis that have been checked to be signed.</returns>
        public int CountRcisTobeSigned()
        {
            return webDriver.FindElements(checkedInputBox).Count;
        }





    }
}
