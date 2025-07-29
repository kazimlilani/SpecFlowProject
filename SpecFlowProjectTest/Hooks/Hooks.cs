using BoDi;
using SpecFlowProjectTest.Support;
using SpecFlowProjectTest.Constants;
using SpecFlowProjectTest.Pages;
using NUnit.Framework;
using SpecFlowProjectTest.Pages.DHCW;
using SpecFlowProjectTest.Tests;

namespace SpecFlowProjectTest.Hooks
{
    [Binding]
    public class Hooks
    {
        private IObjectContainer _objectContainer;
        private static Browser? browser;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeFeature()
        {
            var config = ConfigData.Get();
            CheckConfigVariables(config);

            browser = new Browser(config.Browser, config.Headless, config.BaseUrl);

            var homePage = browser.NavigateTo<DHCW_HomePage>(PageUrl.HOME_PAGE);
        }

        [BeforeFeature(Order = 1)]
        public static void Initialize(IObjectContainer objectContainer)
        {
            Hooks obj = new Hooks(objectContainer);
            obj._objectContainer.RegisterInstanceAs(browser);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            browser?.InterceptorInstance().StopMonitoring();
            browser?.InterceptorInstance().Dispose();
        }

        [AfterTestRun]
        public static void Cleanup()
        {
            browser?.Dispose();
        }

        private static void CheckConfigVariables(ConfigDataModel config)
        {
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(config.Browser, "Missing browser type.");
                Assert.IsNotEmpty(config.BaseUrl, "Missing base url.");
            });
        }

    }
}

