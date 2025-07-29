using AngleSharp.Dom;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;

namespace SpecFlowProjectTest.Support
{
    public abstract class PageAction
    {
        private readonly Browser _browser;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string findElementFailedMessage = "Failed to find the element!";

        public PageAction(Browser browser)
        {
            _browser = browser;
            _driver = browser.DriverInstance();
            _wait = browser.DriverWaitInstance();
        }

        /// <summary>
        /// Handles click on element and ability to retry when theres is an exception
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="force">It uses javascript executor to permorm click</param>
        /// <param name="retryCount">Number of attempts when the click in unsuccesfull</param>
        /// <param name="timeout">Delay time in seconds before attempting to perform and click</param>
        public void ClickOnElement(By locator, bool force = false, int retryCount = 3, int timeout = 0)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            Thread.Sleep(timeout);

            var res = RetryPolicies.RetryBooleanOrDriverException(false, retryCount).ExecuteAndCapture(() =>
            {
                var result = false;
                int count = 0;
                while (!result && count <= retryCount)
                {
                    try
                    {
                        var element = _driver.FindElement(locator);
                        var isDiplayed = WaitForElementToDisplay(locator, 2);
                        if (isDiplayed)
                        {
                            ScrollIntoView(element);
                            if (force)
                            {
                                _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
                                element.ForceClick(_driver);
                            }
                            else
                            {
                                element.Click();
                            }
                            result = true;
                        }
                        else
                        {
                            Console.WriteLine("Not displayed!");
                            result = false;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Hit exception => " + e.GetType().Name.ToUpper());
                        throw;
                    }
                    count++;
                }

                return result;

            });
            res.Result.Should().BeTrue("Failed to click on the element!");
        }

        // <summary>
        /// Handles click on element and ability to retry when theres is an exception
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="force">It uses javascript executor to permorm click</param>
        /// <param name="retryCount">Number of attempts when the click in unsuccesfull</param>
        /// <param name="timeout">Delay time in seconds before attempting to perform and click</param>
        public void ClickOnElement(IWebElement element, bool force = false, int retryCount = 3, int timeout = 0)
        {
            Thread.Sleep(timeout);

            var res = RetryPolicies.RetryBooleanOrDriverException(false, retryCount).ExecuteAndCapture(() =>
            {
                var result = false;
                int count = 0;
                while (!result && count <= retryCount)
                {
                    try
                    {
                        ScrollIntoView(element);
                        if (force)
                        {
                            element.ForceClick(_driver);
                        }
                        else
                        {
                            element.Click();
                        }
                        result = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Hit exception => " + e.GetType().Name.ToUpper());
                        throw;
                    }
                    count++;
                }
                return result;
            });
            res.Result.Should().BeTrue("Failed to click on the element!");
        }

        public void MouseClick(By locator)
        {
            var element = _driver.FindElement(locator);
            ScrollIntoView(element);
            RetryPolicies.RetryException(10).Execute(() =>
            {
                try
                {
                    Actions actions = new(_driver);
                    actions.MoveToElement(element);
                    actions.Click(element).Perform();
                }
                catch (Exception e)
                {
                    Actions actions = new(_driver);
                    actions.MoveToElement(element);
                    actions.Click(element).Perform();
                }
            });
        }

        public void ClickWithJs(By locator)
        {
            var element = _driver.FindElement(locator);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", element);
        }

        public void MouseClick(IWebElement element)
        {
            ScrollIntoView(element);
            RetryPolicies.RetryException(10).Execute(() =>
            {
                Actions actions = new(_driver);
                actions.MoveToElement(element);
                actions.Click(element).Perform();
            });
        }

        public void ClickAway()
        {
            var action = new Actions(_driver);
            action.MoveToElement(_driver.FindElement(By.TagName("body"))).MoveByOffset(0, 0).Click().Perform();
        }

        /// <summary>
        /// Handles right click on element and ability to retry when theres is an exception
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="retryCount">Number of attempts when the click in unsuccesfull</param>
        /// <param name="expectedLocator">Element to expect once the action is completed</param>
        public void RightClickOnElement(By locator, int retryCount = 30, By? expectedLocator = null)
        {
            RetryPolicies.RetryBooleanOrDriverException(false, retryCount).Execute(() =>
            {
                var result = false;
                try
                {
                    WaitForElementToDisplay(locator, 2);
                    result = true;
                }
                catch (Exception)
                {

                    throw;
                }
                return result;
            });

            if (expectedLocator is not null)
            {
                var isExpectedDisplayed = false;

                for (int i = 0; i < 3; i++)
                {
                    if (isExpectedDisplayed)
                    {
                        break;
                    }
                    try
                    {
                        WaitForElementToDisplay(expectedLocator, 2);
                        isExpectedDisplayed = true;
                    }
                    catch (Exception)
                    {

                        var element2 = _driver.FindElement(locator);
                        Actions actions = new(_driver);
                        actions.ContextClick(element2).Perform();
                    }

                }
            }

            else
            {
                RetryPolicies.RetryException(5).Execute(() =>
                {
                    var element = _driver.FindElement(locator);
                    Actions actions = new(_driver);
                    actions.ContextClick(element).Perform();
                });
            }
        }

        /// <summary>
        /// Handles Enter text to the element with waiting mechanism
        /// Text will only be entered when the element is displayed
        /// It will ignore exception until the given timeout is reached
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="text">Text value to enter</param>
        /// <param name="timeout">Maximum time in seconds to wait util the element is displayed</param>
        public void EnterText(By locator, string text, int timeout = 30)
        {
            WaitForElementToDisplay(locator, timeout).Should().BeTrue(findElementFailedMessage);
            ScrollIntoView(locator);

            RetryPolicies.RetryException().Execute(() =>
            {
                _driver.FindElement(locator).Clear();
                _driver.FindElement(locator).SendKeys(text);
            });
        }

        /// <summary>
        /// Handles Pressing tab from the element with waiting mechanism
        /// Tab button press will only be implemented when the element is displayed
        /// It will ignore exception until the given timeout is reached
        /// </summary>
        /// <param name="locator">Element locator</param>
        ///  <param name="timeout">Maximum time in seconds to wait util the element is displayed</param>
        public void PressTab(By locator, int timeout = 30)
        {
            WaitForElementToDisplay(locator, timeout).Should().BeTrue(findElementFailedMessage);
            ScrollIntoView(locator);
            RetryPolicies.RetryException().Execute(() =>
            {
                _driver.FindElement(locator).SendKeys(Keys.Tab);
            });
        }

        /// <summary>
        /// Handles capture text from the element by Locator
        /// It will wait for element to exist before performing the action
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeout">Maximum time in seconds to wait util the element is exist</param>
        /// <returns></returns>
        public string GetText(By locator, int timeout = 15)
        {
            WaitForElementToExist(locator, timeout).Should().BeTrue();
            ScrollIntoView(locator);
            return _driver.FindElement(locator).Text;
        }

        /// <summary>
        /// Handles capture text from the element by WebElement
        /// It will wait for loader to disappear (if exists)
        /// It will wait for element to display before performing the action
        /// </summary>
        /// <param name="element">Web Element</param>
        /// <param name="timeout">Maximum time in seconds to wait until the element is displayed</param>
        /// <returns></returns>
        public string GetText(IWebElement element, int timeout = 15)
        {
            FluentWait(timeout).Until((d) => element.Displayed);
            return element.Text;
        }

        /// <summary>
        /// Handles capture value of an input field
        /// It waits for the element to exist and scroll to the element before performing the action
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns></returns>
        public string GetTextByAttributeValue(By locator)
        {
            WaitForElementToExist(locator).Should().BeTrue(findElementFailedMessage);
            ScrollIntoView(locator);
            return GetElement(locator).GetAttribute("value");
        }

        public string GetTextByAttributeValue(IWebElement element)
        {
            ScrollIntoView(element);
            return element.GetAttribute("value");
        }
        /// <summary>
        /// Handles select option from the dropdown by text
        /// It will wait for element to display and retry when exception occur
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="text">Displayed text on the dropdown</param>
        public void SelectOptionByText(By locator, string text)
        {
            WaitForElementToDisplay(locator).Should().BeTrue(findElementFailedMessage);
            var selectClass = new SelectElement(GetElement(locator));
            RetryPolicies.RetryException().Execute(() =>
            {
                selectClass.SelectByText(text);
            });
        }

        public void SelectOptionByText(IWebElement element, string text)
        {
            var selectClass = new SelectElement(element);
            RetryPolicies.RetryException().Execute(() =>
            {
                selectClass.SelectByText(text);
            });
        }

        /// <summary>
        /// Handles select option from the dropdown by text
        /// It will wait for element to display and retry when exception occur
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="text">Displayed text on the dropdown</param>
        /// <param name="partialMatch">Match text partially</param>

        public void SelectOptionByText(By locator, string text, bool partialMatch)
        {
            WaitForElementToDisplay(locator).Should().BeTrue(findElementFailedMessage);
            WaitForDropdownToPopulate(locator);
            var selectClass = new SelectElement(GetElement(locator));
            RetryPolicies.RetryException().Execute(() =>
            {
                selectClass.SelectByText(text, partialMatch);
            });
        }

        /// <summary>
        /// Handles select option from the dropdown by value
        /// It will wait for element to display and retry when exception occur
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="value">Option value on the dropdown</param>
        public void SelectOptionByValue(By locator, string value)
        {
            WaitForElementToDisplay(locator).Should().BeTrue(findElementFailedMessage);
            var selectClass = new SelectElement(GetElement(locator));
            RetryPolicies.RetryException().Execute(() =>
            {
                selectClass.SelectByValue(value);
            });
        }

        /// <summary>
        /// Handles select option from the dropdown by index
        /// It will wait for element to display and retry when exception occur
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="value">Index value of the dropdown</param>
        public void SelectOptionByIndex(By locator, int index)
        {
            WaitForElementToDisplay(locator).Should().BeTrue(findElementFailedMessage);
            var selectClass = new SelectElement(GetElement(locator));
            RetryPolicies.RetryException().Execute(() =>
            {
                selectClass.SelectByIndex(index);
            });
        }

        /// <summary>
        /// Handles capture selected option from the dropdown
        /// It will wait for element to display before performing the action
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns></returns>
        public string SelectedDropdownOptionText(By locator)
        {
            WaitForElementToDisplay(locator).Should().BeTrue(findElementFailedMessage);
            var selectClass = new SelectElement(GetElement(locator));
            return selectClass.SelectedOption.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string SelectedDropdownOptionText(IWebElement element)
        {
            FluentWait(15).Until(d => element.Displayed).Should().BeTrue("Failed to find element!");
            var selectClass = new SelectElement(element);
            return selectClass.SelectedOption.Text;
        }

        /// <summary>
        /// It will wait for element to display before performing the action
        /// Selects the dropdown element
        /// Returns the dropdown options form the selected Dropdown
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns></returns>
        public List<string> DropdownOptions(By locator)
        {
            var element = _driver.FindElement(locator);
            ScrollIntoView(element);
            var selectClass = new SelectElement(element);
            List<string> actualDropdownValues = new List<string>();

            List<string> Action()
            {
                foreach (IWebElement option in selectClass.Options)
                {
                    actualDropdownValues.Add(option.Text.Trim());
                }
                actualDropdownValues = actualDropdownValues.Where(option => !IsOptionDisabled(option, element)).ToList();
                return actualDropdownValues;
            }
            return RetryPolicies.RetryException(3).Execute(Action);
        }

        /// <summary>
        /// Return is the dropdown Option is Disabled or not </summary>
        /// <param string="optionText">DropdownOptionText</param>
        /// <param IWebElement="dropdownElement">DropdownElement</param>
        /// <returns></returns>
        private bool IsOptionDisabled(string optionText, IWebElement dropdownElement)
        {
            var selectClass = new SelectElement(dropdownElement);
            var optionElement = selectClass.Options.FirstOrDefault(option => option.Text.Trim() == optionText);
            return optionElement != null && optionElement.GetAttribute("disabled") == "true";
        }

        public void ScrollIntoView(By elementSelector)
        {
            var element = _driver.FindElement(elementSelector);
            int bannerHeight = 500;
            ((IJavaScriptExecutor)_driver).ExecuteScript(@"
                    const element = arguments[0];
                    const offset = arguments[1];
                    const topPosition = element.getBoundingClientRect().top + window.scrollY - offset;
                    window.scrollTo({ top: topPosition, behavior: 'smooth' });", element, bannerHeight);

        }

        public void ScrollIntoView(IWebElement element)
        {
            int bannerHeight = 500;
            ((IJavaScriptExecutor)_driver).ExecuteScript(@"
                    const element = arguments[0];
                    const offset = arguments[1];
                    const topPosition = element.getBoundingClientRect().top + window.scrollY - offset;
                    window.scrollTo({ top: topPosition, behavior: 'smooth' });", element, bannerHeight);

        }

        public void ScrollToBottom()
        {
            ((IJavaScriptExecutor)_driver)
         .ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        }

        public void ScrollToTop()
        {
            ((IJavaScriptExecutor)_driver)
         .ExecuteScript("window.scrollTo(0, 0);");
        }

        /// <summary>
        /// Handles capture color of element
        /// It will wait until element is displayed before performing the action
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeout">Maximum time in seconds to wait until element is displayed</param>
        /// <returns></returns>
        public string GetColorOfText(By locator, int timeout = 15)
        {
            WaitForElementToDisplay(locator, timeout).Should().BeTrue(findElementFailedMessage);
            return GetElement(locator).GetCssValue("color");
        }

        private IAlert SwitchToAlert()
        {
            return _driver.SwitchTo().Alert();
        }

        /// <summary>
        /// Handles alert select option
        /// It will click the option as per the text param
        /// </summary>
        /// <param name="text">Option value to click</param>
        /// <param name="accept">Option to accept of not</param>

        public void AcceptOrDismissAlert(string text, bool accept)
        {
            IAlert alert = SwitchToAlert();

            if (alert.Text.Equals(text, StringComparison.CurrentCultureIgnoreCase))
            {
                if (accept)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
            }
        }

        /// <summary>
        /// Handles Get webElement by locator with maximum waiting timeout
        /// It will wait until element exists before performin the action
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeout">Maximum time in seconds until it exists</param>
        /// <returns></returns>

        public IWebElement GetElement(By locator, int timeout = 15)
        {
            WaitForElementToExist(locator).Should().BeTrue(findElementFailedMessage);
            return FluentWait(timeout).Until(d => d.FindElement(locator));
        }

        /// <summary>
        /// Handles Get webElements by locator with maximum waiting timeout
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeOut">Maximum time in seconds before performing the action</param>
        /// <returns></returns>
        public List<IWebElement> GetElements(By locator, int timeOut = 15)
        {
            return FluentWait(timeOut).Until(d => d.FindElements(locator).ToList());
        }

        public void ClickBrowsersBackButton()
        {
            _driver.Navigate().Back();
        }

        /// <summary>
        /// Handles wait for element to display by locator with maximum waiting timeout 
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeOut">Maximum time in seconds before performing the action</param>
        /// <returns></returns>
        public bool WaitForElementToDisplay(By locator, int timeout = 30)
        {
            return FluentWait(timeout).Until(d => d.FindElement(locator).Displayed);
        }

        public bool WaitForElementToDisplayWithRetry(By locator, int timeout = 5, int retryCount = 3)
        {
            int count = 1;
            bool displayed = false;
            while (count <= retryCount)
            {
                displayed = FluentWait(timeout).Until(d => d.FindElement(locator).Displayed);
            }

            return displayed;
        }

        /// <summary>
        /// Handles wait for dropdown to populate with options 
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeOut">Maximum time in seconds before performing the action</param>
        /// <returns></returns>

        public bool WaitForDropdownToPopulate(By locator, int timeout = 30)
        {
            return FluentWait(timeout).Until(d =>
            {
                var dropdown = d.FindElement(locator);
                var selectElement = new SelectElement(dropdown);
                return selectElement.Options.Count > 1;

            });
        }

        /// <summary>
        /// Evaluate if element is displayed
        /// It will retry if the element encounters stale exception
        /// </summary>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public bool IsElementDisplayed(By elementSelector)
        {
            try
            {
                ScrollIntoView(elementSelector);
                return _driver.FindElement(elementSelector).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.Message.Contains("Element to be searched not found"))
                {
                    return false;
                }
                return IsElementDisplayed(elementSelector);
            }
            catch (StaleElementReferenceException)
            {
                return IsElementDisplayed(elementSelector);
            }
        }

        public bool DoesElementExist(By elementSelector)
        {
            try
            {
                _driver.FindElement(elementSelector);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsElementDisplayed(IWebElement element)
        {
            try
            {
                ScrollIntoView(element);
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException ex)
            {
                if (ex.Message.Contains("Element to be searched not found"))
                {
                    return false;
                }
                return IsElementDisplayed(element);
            }
            catch (StaleElementReferenceException)
            {
                return IsElementDisplayed(element);
            }
        }


        /// <summary>
        /// Evaluate if Text from element is Bold
        /// It will wait for element to exist before performing the action
        /// </summary>
        /// <param name="elementlocator">Element locator</param>
        /// <returns></returns>
        public bool IsElementTextBold(By elementlocator)
        {
            WaitForElementToExist(elementlocator).Should().BeTrue(findElementFailedMessage);
            string fontWeight = _driver.FindElement(elementlocator).GetCssValue("font-weight");
            if (int.TryParse(fontWeight, out int weight) && weight >= 600)
            {
                return true;
            }
            else if (fontWeight.Equals("bold", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Evaluate if element is enabled be locator
        /// It will wait for element to exist before performing the action
        /// </summary>
        /// <param name="elementlocator">Element locator</param>
        /// <returns></returns>
        public bool IsElementEnabled(By elementlocator)
        {
            WaitForElementToExist(elementlocator).Should().BeTrue(findElementFailedMessage);
            return FluentWait(15).Until(d => d.FindElement(elementlocator).Enabled);
        }

        public bool IsElementEnabled(IWebElement element)
        {
            return element.Enabled;
        }

        /// <summary>
        /// Evaluate if element is clickable or not
        /// It will wait for element to exist before performing the action
        /// </summary>
        /// <param name="elementlocator">Element locator</param>
        /// <returns></returns>
        public bool IsElementClickable(By elementlocator)
        {
            try
            {
                WaitForElementToExist(elementlocator).Should().BeTrue(findElementFailedMessage);
                _wait.Until(ExpectedConditions.ElementToBeClickable(elementlocator));
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public bool IsElementClickable(IWebElement element)
        {
            try
            {
                _wait.Until(ExpectedConditions.ElementToBeClickable(element));
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Evaluate if element is disabled by locator
        /// It will wait for element to exist before performing the action
        /// </summary>
        /// <param name="elementlocator">Element locator</param>
        /// <returns></returns>
        public bool IsElementDisabled(By elementlocator)
        {
            WaitForElementToExist(elementlocator).Should().BeTrue(findElementFailedMessage);
            return FluentWait(15).Until(d => !d.FindElement(elementlocator).Enabled);
        }

        /// <summary>
        /// Evaluate if element is disabled by WebElement
        /// </summary>
        /// <param name="element">WebElement</param>
        /// <returns></returns>
        public bool IsElementDisabled(IWebElement element)
        {
            if (element.GetAttribute("class").Contains("disabled") || element.Enabled == false)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Evaluate if element is selected or not
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeout">Maximum time in seconds to wait until element is displayed</param>
        /// <returns></returns>
        public bool IsElementSelected(By locator, int timeout = 15)
        {
            return GetElement(locator).Selected;
        }

        /// <summary>
        /// Evaluate if element is selected or not
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns></returns>
        public bool IsElementSelectedByClass(IWebElement element)
        {
            if (element.GetAttribute("class").Contains("selected") || element.Enabled == false)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Evaluate if element is selected or not
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns></returns>
        public bool IsElementUnSelectedByClass(IWebElement element)
        {
            if (element.GetAttribute("class").Contains("unselected") || element.Enabled == false)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Evaluate if element is selected by WebElement
        /// </summary>
        /// <param name="element">WebElement</param>
        /// <param name="timeout">Maximum time in seconds to wait until element is selected</param>
        /// <returns></returns>
        public bool IsElementSelected(IWebElement element, int timeout = 15)
        {
            return FluentWait(timeout).Until(_ => element.Selected);
        }


        /// <summary>
        /// handles wait for element to exist with wating timeout 
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <param name="timeOut">Maximum time in seconds until the wait condition is met</param>
        /// <returns></returns>
        public bool WaitForElementToExist(By locator, int timeout = 30)
        {
            return FluentWait(timeout).Until(d => d.FindElements(locator).Count > 0);
        }

        public IWebElement WaitForElementToBeClickable(By locator, int timeout = 30)
        {
            return FluentWait(timeout).Until(d => _wait.Until(ExpectedConditions.ElementToBeClickable(locator)));
        }

        public bool IsElementExist(By elementSelector)
        {
            try
            {
                return _driver.FindElements(elementSelector).Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return IsElementExist(elementSelector);
            }
        }

        public DefaultWait<IWebDriver> FluentWait(int timeout)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(_driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(timeout);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            fluentWait.Message = "Element to be searched not found";

            return fluentWait;
        }
    }
}

