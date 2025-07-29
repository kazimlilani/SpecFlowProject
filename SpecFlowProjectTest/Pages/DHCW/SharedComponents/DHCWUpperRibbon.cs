using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpecFlowProjectTest.Support;

namespace SpecFlowProjectTest.Pages.DHCW.Shared_Components
{
    internal class DHCWUpperRibbon : PageAction
    {

        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly Browser _browser;
        public DHCWUpperRibbon(Browser browser) : base(browser)
        {
            _browser = browser;
            _driver = browser.DriverInstance();
            _wait = browser.DriverWaitInstance();
        }

        #region Locators

        By HomeButton = By.XPath("//a[contains(@class, 'nav-link') and contains(text(), 'Home')]");
        By NewsButton = By.XPath("//a[contains(@class, 'nav-link') and contains(text(), 'News')]");

        #endregion

        #region Actions

        public void ClickHomeButton()
        {
            MouseClick(HomeButton);
        }

        public void ClickNewsButton()
        {
            MouseClick(NewsButton);
        }

        #endregion

        #region Assertions and Checks

        public void HomeButtonIsPresent(IWebDriver driver)
        {
            IsElementDisplayed(HomeButton).Should().BeTrue();
        }

        #endregion

    }
}
