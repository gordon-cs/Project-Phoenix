using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Phoenix.Tests.TestUtilities;

namespace Phoenix.Tests.Pages
{
    public class LoginPage
    {
        private IWebDriver webDriver;

        private static By usernameInput = By.CssSelector("[data-selenium-id='login-page-username-input']");
        private static By passwordInput = By.CssSelector("[data-selenium-id='login-page-password-input']");
        private static By submitButton = By.CssSelector("[data-selenium-id='login-page-submit-button']");

        public LoginPage(IWebDriver driver)
        {
            webDriver = driver;

            // If this is the login page, the Username input must be visible
           try
            {
                new WebDriverWait(webDriver, Values.DEFAULT_WAIT).Until(ExpectedConditions.ElementIsVisible(usernameInput));
            }
            catch(WebDriverTimeoutException)
            {
                throw new IllegalStateException(string.Format("This is not the Login page. Current page title: {0}.\n Url: {1}", webDriver.Title, webDriver.Url));
            }
        }

        /// <summary>
        /// Input the username into the username input box
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>This Loginpage object</returns>
        public LoginPage EnterUsername(string username)
        {
            webDriver.FindElement(usernameInput).SendKeys(username);

            return this;
        }

        /// <summary>
        /// Input the password into the password input box
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>This LoginPage object</returns>
        public LoginPage EnterUserPassword(string password)
        {
            webDriver.FindElement(passwordInput).SendKeys(password);

            return this;
        }

        /// <summary>
        /// Click on the "Login" button
        /// </summary>
        /// <returns>A DashboardPage object</returns>
        public DashboardPage SubmitCredentials()
        {
            webDriver.FindElement(submitButton).Submit();

            return new DashboardPage(webDriver) ;
        }

        /// <summary>
        /// If you have no need for going through the discreet steps; enter username, password then submit,
        /// You can use this method to login.
        /// </summary>
        /// <param name="username">Login username</param>
        /// <param name="password">Login password</param>
        /// <returns>A DashboardPage object</returns>
        public DashboardPage LoginAs(string username, string password)
        {
            EnterUsername(username);
            EnterUserPassword(password);
            return SubmitCredentials();
        }
    }
}
