using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SpecFlowProjectTest.Pages.DHCW.Shared_Components;
using SpecFlowProjectTest.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowProjectTest.Pages.DHCW
{
    internal class DHCW_HomePage : DHCWUpperRibbon
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly Browser _browser;
        public DHCW_HomePage(Browser browser) : base(browser)
        {
            _browser = browser;
            _driver = browser.DriverInstance();
            _wait = browser.DriverWaitInstance();
        }

        #region Locators

        By DHCWMottoHeader = By.XPath("//h2[contains(text(), 'Making digital a force for good in health and care')]");


        #endregion

        #region Actions


        #endregion

        #region Assertions and Checks

        internal void DHCWMottoIsPresent()
        {
            WaitForElementToDisplay(DHCWMottoHeader).Should().BeTrue();
        }

        #endregion

    }
}
