using OpenQA.Selenium;
using Phoenix.Tests.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Tests.Models
{
    public class RciCard
    {
        private IWebElement element;
        private IWebDriver webDriver;
        private string rciCheckinClass = "rci-card-checkin";
        private string rciCheckoutClass = "rci-card-checkout";

        private static By rciLink = By.CssSelector("[data-selenium-id='dashboard-page-rci-button']");
        private static By rciSignatureBlocks = By.CssSelector("[data-selenium-id='dashboard-page-signature-blocks']");

        public RciCard(IWebElement e, IWebDriver wd)
        {
            element = e;
            webDriver = wd;
        }

        public int GetId()
        {
            return int.Parse(element.GetAttribute("id"));
        }

        public bool isCheckinRci()
        {
            return element.FindElement(By.ClassName(rciCheckinClass)) != null;
        }

        public bool isCheckoutRci()
        {
            return element.FindElement(By.ClassName(rciCheckoutClass)) != null;
        }

        public bool isUnsigned()
        {
            return element.FindElement(rciSignatureBlocks).FindElements(By.TagName("span")).Count == 0;
        }

        public bool isSignedByResident()
        {
            return element.FindElement(rciSignatureBlocks).Text.Contains("RES");
        }

        public bool isSignedByRA()
        {
            return element.FindElement(rciSignatureBlocks).Text.Contains("RA");
        }

        public bool isSignedByRD()
        {
            return element.FindElement(rciSignatureBlocks).Text.Contains("RD");
        }

        public GenericRciPage Click()
        {
            element.FindElement(rciLink).Click();

            return new GenericRciPage(webDriver);
        }
    }
}
