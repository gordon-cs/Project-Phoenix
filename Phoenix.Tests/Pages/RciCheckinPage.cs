using OpenQA.Selenium;
using Phoenix.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.Pages
{
    public class RciCheckinPage : GenericRciPage
    {
        private static By signatureContainer = By.Id("signature-container");
        private static By signatureInput = By.CssSelector("[data-selenium-id='signature-field']");
        private static By signatureSubmit = By.Id("submit-button");
        private static By queueForSigningCheckbox = By.Id("rci-sig-checkbox");

        public bool isEditPage;
        public bool isReviewPage;

        public RciCheckinPage(IWebDriver driver)
            : base(driver)
        {
            // Verify that we are indeed on a RciCheckinPage

            isEditPage = Methods.VerifyWeAreOnPageWithTitle(webDriver, Values.RCI_CHECKIN_PAGE_EDIT_TITLE);
            isReviewPage = Methods.VerifyWeAreOnPageWithTitle(webDriver, Values.RCI_CHECKIN_PAGE_REVIEW_TITLE);

            if(!(isEditPage || isReviewPage))
            {
                throw new IllegalStateException(string.Format("We are not on a Checkin page! Current page title {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }

        }

        /// <summary>
        /// Click on the "next" button.
        /// </summary>
        /// <returns>The current RciCheckinPage object</returns>
        /// <exception cref="IllegalFunctionCall">If the "next" button was not found</exception>
        public RciCheckinPage HitNextToSignatures()
        {
            try
            {
                webDriver.FindElement(nextButton).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new IllegalFunctionCall("The \"Next\" button was not found. This might be a review page.", e);
            }

            return this;
        }

        /// <summary>
        /// Fill every input box in the signature page with the provided signature
        /// </summary>
        /// <param name="signature">The text with which to fill the input boxes</param>
        /// <exception cref="IllegalStateException">If not signature input boxes exist</exception>
        public RciCheckinPage Sign(string signature)
        {

            var inputBoxes = webDriver.FindElements(signatureInput);

            // When we call Sign(), we expect that there will signature boxes. 
            // If none exist, this means that the current user doesn't have the right to sign said rci.
            if (inputBoxes.Count() < 1)
            {
                throw new IllegalStateException("Expected to find input boxes, but none were found");
            }

            foreach(var inputBox in inputBoxes)
            {
                inputBox.SendKeys(signature);
            }

            return this;
        }

        /// <summary>
        /// Submit the signature by clickin the submit button
        /// </summary>
        /// <returns>A new DashboardPage object</returns>
        /// <exception cref="IllegalStateException">The button to submit signatures could not be found.</exception>
        public DashboardPage SubmitSignature()
        {
            try
            {
                webDriver.FindElement(signatureSubmit).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new NoSuchElementException("Could not click on the button to submit the signature(s)", e);
            }

            return new DashboardPage(webDriver);
            
        }

        /// <summary>
        /// Returns a string representing the text inside the signature popup
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException">The signature popup was not open or visible</exception>
        public string GetSignaturePagePopupText()
        {
            try
            {
                return webDriver.FindElement(signatureContainer).Text;
            }
            catch(NoSuchElementException e)
            {
                throw new NoSuchElementException("Could not find the signature popup. Perhaps it has not yet been opened/is not visible.", e);
            }
        }

        public RciCheckinPage ToggleQueueForSigningCheckbox()
        {
            IWebElement checkbox;
            try
            {
                checkbox = webDriver.FindElement(queueForSigningCheckbox);
            }
            catch(NoSuchElementException e)
            {
                throw new NoSuchElementException("Could not find the checkbox allowing an RD to queue an RCI for later signing. Perhaps this person is not an RD.", e);
            }

            checkbox.Click();

            return this;
        }
    }
}
