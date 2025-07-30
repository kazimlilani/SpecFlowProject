using BoDi;
using SpecFlowProjectTest.Support;
using SpecFlowProjectTest.Constants;
using SpecFlowProjectTest.Pages;
using NUnit.Framework;
using SpecFlowProjectTest.Pages.DHCW;
using SpecFlowProjectTest.Tests;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Serilog;

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
            // Serilog initialization
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\automation.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Test run started.");

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
            Log.Information("Test run finished.");
            browser?.Dispose();
            Log.CloseAndFlush();
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

    [Binding]
    public class ExtentHooks
    {
        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            Log.Information("Starting scenario: {Scenario}", scenarioContext.ScenarioInfo.Title);
            ExtentReportHelper.Test = ExtentReportHelper.Extent.CreateTest(scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            var stepInfo = scenarioContext.StepContext.StepInfo;
            if (scenarioContext.TestError == null)
            {
                Log.Information("Step passed: {Step}", stepInfo.Text);
                ExtentReportHelper.Test.Pass(stepInfo.Text);
            }
            else
            {
                Log.Error("Step failed: {Step} - {Error}", stepInfo.Text, scenarioContext.TestError.Message);
                ExtentReportHelper.Test.Fail($"{stepInfo.Text} - {scenarioContext.TestError.Message}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            ExtentReportHelper.FlushReport();
        }
    }
}

