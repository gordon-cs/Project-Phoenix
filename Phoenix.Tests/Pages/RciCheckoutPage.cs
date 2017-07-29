using OpenQA.Selenium;
using Phoenix.Tests.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.Pages
{
    public class RciCheckoutPage : GenericRciPage
    {
        public static By checkoutSignatureNextButton = By.CssSelector("[data-selenium-id='checkout-signature-next-button']");
        public static By signatureInput = By.CssSelector("[data-selenium-id='signature-field']");
        public static By usernameInput = By.CssSelector("input[name='username']");
        public static By passwordInput = By.CssSelector("input[name='password']");

        public RciCheckoutPage(IWebDriver driver)
            : base(driver)
        {
            // Verify that we are indeed on a RciCheckoutPage

            bool isEditPage = Methods.VerifyWeAreOnPageWithTitle(webDriver, Values.RCI_CHECKOUT_PAGE_EDIT_TITLE);
            bool isReviewPage = Methods.VerifyWeAreOnPageWithTitle(webDriver, Values.RCI_CHECKOUT_PAGE_REVIEW_TITLE);

            if (!(isEditPage || isReviewPage))
            {
                throw new IllegalStateException(string.Format("We are not on a Checkout page! Current page title {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }
        }

        /// <summary>
        /// Click on the "next" button from the rci input page
        /// </summary>
        /// <returns>The current RciCheckoutPage object</returns>
        /// <exception cref="IllegalFunctionCall">If the "next" button was not found</exception>
        public RciCheckoutPage HitNextToResidentSignature()
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
        /// Advance to the RA signature page.
        /// </summary>
        /// <returns>The current RciCheckoutPage object</returns>
        /// <exception cref="IllegalStateException">If not signature input boxes exist</exception>
        public RciCheckoutPage HitNextToRASignature()
        {
            try
            {
                webDriver.FindElement(checkoutSignatureNextButton).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new IllegalFunctionCall("The \"Next\" button was not found. You are probably already back on the Dashboard Page", e);
            }
            return this;
        }

        /// <summary>
        /// Advance to the RD signature page
        /// </summary>
        /// <returns>The current RciCheckout page object</returns>
        /// <exception cref="IllegalFunctionCall">If no "next" button was found</exception>
        public RciCheckoutPage HitNextToRDSignature()
        {
            try
            {
                webDriver.FindElement(checkoutSignatureNextButton).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new IllegalFunctionCall("The \"Next\" button was not found. You are probably already back on the Dashboard Page", e);
            }
            return this;
        }

        /// <summary>
        /// Advance to the dashboard page.
        /// </summary>
        /// <returns>A new DashboardPage object</returns>
        /// <exception cref="IllegalFunctionCall">If no "next" button was found</exception>
        public DashboardPage HitNextToDashboard()
        {
            try
            {
                webDriver.FindElement(checkoutSignatureNextButton).Click();
            }
            catch(NoSuchElementException  e)
            {
                throw new IllegalFunctionCall("The \"Next\" button was not found. You are probably already back on the Dashboard Page", e);
            }
            return new DashboardPage(webDriver);
        }

        /// <summary>
        /// Fill every signature input box in the signature page with the provided signature
        /// </summary>
        /// <param name="signature">The text with which to fill the input boxes</param>
        /// <exception cref="IllegalStateException">If not signature input boxes exist</exception>
        public RciCheckoutPage Sign(string signature)
        {

            var inputBoxes = webDriver.FindElements(signatureInput);

            // When we call Sign(), we expect that there will signature boxes. 
            // If none exist, this means that the current user doesn't have the right to sign said rci.
            if (inputBoxes.Count() < 1)
            {
                throw new IllegalStateException("Expected to find input boxes, but none were found");
            }

            foreach (var inputBox in inputBoxes)
            {
                inputBox.SendKeys(signature);
            }

            return this;
        }


        /// <summary>
        /// Enter username and password as a form of signature.
        /// </summary>
        /// <param name="username">The username of the person authenticating</param>
        /// <param name="password">The password of the person authenticating</param>
        /// <returns>A new DashboardPage object</returns>
        public RciCheckoutPage Sign(string username, string password)
        {
            webDriver.FindElement(usernameInput).Clear();
            webDriver.FindElement(usernameInput).SendKeys(username);

            webDriver.FindElement(passwordInput).SendKeys(password);

            return this;
        }

        /// <summary>
        /// Submit the signature by clicking the next button
        /// </summary>
        /// <returns>A new DashboardPage object</returns>
        public DashboardPage SubmitSignature()
        {
            try
            {
                webDriver.FindElement(checkoutSignatureNextButton).Click();
            }
            catch (NoSuchElementException e)
            {
                throw new IllegalStateException("Could not click on the next button to submit the signature(s)", e);
            }
            return new DashboardPage(webDriver);
        }

    }
}
