using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Phoenix.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private IWebDriver wd = new FirefoxDriver();

        [TestMethod]
        public void TestMethod1()
        {
            wd.Navigate().GoToUrl("https://rci.gordon.edu");


        }
    }
}
