using OpenQA.Selenium;
using Phoenix.Tests.TestUtilities;

namespace Phoenix.Tests.Pages
{
    public class GenericRciPage
    {
        protected IWebDriver webDriver;

        private static By rciComponents = By.ClassName("component");
        protected static By nextButton = By.Id("next-button");

        public GenericRciPage(IWebDriver driver)
        {
            webDriver = driver;
            if(!
                (Methods.VerifyWeAreOnPageWithTitle(driver, Values.RCI_CHECKIN_PAGE_EDIT_TITLE) || 
                Methods.VerifyWeAreOnPageWithTitle(driver, Values.RCI_CHECKIN_PAGE_REVIEW_TITLE) ||
                Methods.VerifyWeAreOnPageWithTitle(driver, Values.RCI_CHECKOUT_PAGE_EDIT_TITLE) ||
                Methods.VerifyWeAreOnPageWithTitle(driver, Values.RCI_CHECKOUT_PAGE_REVIEW_TITLE)))
            {
                // If this is not one of the four types of Rci pages.
                throw new IllegalStateException(string.Format("This is not an Rci page. Current page title: {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }
        }

        /// <summary>
        /// The GenericRciPage object is, well, generic. We use it when we don't care
        /// what kind of page we get when we click  on an Rci from the dashboard.
        /// When we do care what kind of page we get, we  need to convert it to a more 
        /// specific type. So if we knew we were going to get a checkout page, we would 
        /// use this method to "convert" the GenericRciPage object we have to the more
        /// specific RciCheckoutpage.
        /// </summary>
        /// <returns>The RciCheckoutPage object</returns>
        /// <exception cref="IllegalStateException">If the page we are on isn't actually a checkout page</exception>"
        public RciCheckoutPage asRciCheckoutPage()
        {
            return new RciCheckoutPage(webDriver);
        }

        /// <summary>
        /// The GenericRciPage object is, well, generic. We use it when we don't care
        /// what kind of page we get when we click  on an Rci from the dashboard.
        /// When we do care what kind of page we get, we  need to convert it to a more 
        /// specific type. So if we knew we were going to get a checkin page, we would 
        /// use this method to "convert" the GenericRciPage object we have to the more
        /// specific RciCheckinPage.
        /// </summary>
        /// <returns>The RciCheckin object</returns>
        /// <exception cref="IllegalStateException">If the page we are on isn't actually a checkin page</exception>"
        public RciCheckinPage asRciCheckinPage()
        {
            return new RciCheckinPage(webDriver);
        }

        /// <summary>
        /// Count the number of rci components present.
        /// </summary>
        /// <returns>The number of visible rci components</returns>
        public int ComponentCount()
        {
            return webDriver.FindElements(rciComponents).Count;
        }
    } 
}
