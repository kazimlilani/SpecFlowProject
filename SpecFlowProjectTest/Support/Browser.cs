using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using SpecFlowProjectTest.Enums;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace SpecFlowProjectTest.Support
{
    public class Browser : IDisposable
    {
        private IWebDriver _driver;
        private readonly Interceptor _interceptor;
        private readonly bool _isHeadless;
        private readonly string _baseUrl;
        public Browser(string browserType, bool isHeadless, string baseUrl)
        {
            _isHeadless = isHeadless;
            _baseUrl = baseUrl;
            SelectBrowser(browserType);
            _driver!.Manage().Window.Maximize();
            _driver!.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            _interceptor = new Interceptor(_driver);
        }

        public void Reload()
        {
            _driver.Navigate().Refresh();
        }
        public T NavigateTo<T>(string url)
        {
            var baseUrl = _baseUrl;
            _driver.Navigate().GoToUrl(baseUrl + url);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            //js.ExecuteScript("document.documentElement.style.zoom='80%'");
            return (T)Activator.CreateInstance(typeof(T), this)!;
        }

        public T NavigateTo<T>()
        {
            return (T)Activator.CreateInstance(typeof(T), this)!;
        }
        public IWebDriver DriverInstance()
        {
            return _driver;
        }
        public WebDriverWait DriverWaitInstance(int timeout = 30)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200)
            };
            wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException), typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(InvalidElementStateException));
            return wait;
        }

        public Interceptor InterceptorInstance()
        {
            return _interceptor;
        }

        public void ClickAway()
        {
            Actions action = new(_driver);
            action.MoveByOffset(0, 0).Click().Build().Perform();
        }
        public void WaitForSaveLoader()
        {
            var spinner = By.Id("modal-Progressing");
            if (_driver.FindElements(spinner).Count > 0)
            {
                DriverWaitInstance().Until(ExpectedConditions.InvisibilityOfElementLocated(spinner));
            }
        }
        private void SelectBrowser(string browser)
        {
            var browserType = (BrowserType)Enum.Parse(typeof(BrowserType), browser.ToUpper());
            var isHeadless = _isHeadless;
            var argumentHeadlessOption = $"{(isHeadless ? "--headless=new" : "headed")}";


            if (browserType == BrowserType.CHROME)
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                ChromeOptions chromeOptions = new();
                chromeOptions.AddArguments(argumentHeadlessOption, "--incognito");
                _driver = new ChromeDriver(chromeOptions);
            }
            else
            {
                new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);
                EdgeOptions edgeOptions = new();
                edgeOptions.AddArguments(argumentHeadlessOption, "--inprivate");
                _driver = new EdgeDriver(edgeOptions);
            }
        }

        public void SetSessionStorage(string key, string value)
        {
            ((IJavaScriptExecutor)_driver)
                .ExecuteScript("sessionStorage.setItem(arguments[0],arguments[1]);", key, value);
        }

        public string? GetSessionStorageItem(string key)
        {
            return (string?)((IJavaScriptExecutor)_driver)
                .ExecuteScript("sessionStorage.getItem(arguments[0]);", key);
        }

        public void RemoveSessionStorageItem(string key)
        {
            if (GetSessionStorageItem(key) is not null)
            {
                ((IJavaScriptExecutor)_driver)
                   .ExecuteScript("sessionStorage.removeItem(arguments[0]);", key);
            }

        }
        public void Dispose()
        {
            _driver?.Quit();
        }
    }
}

