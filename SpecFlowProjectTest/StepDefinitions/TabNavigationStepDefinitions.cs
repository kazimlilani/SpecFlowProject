using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using SpecFlowProjectTest.Constants;
using SpecFlowProjectTest.Pages.DHCW;
using SpecFlowProjectTest.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace SpecFlowProjectTest.StepDefinitions
{
    [Binding]
    internal class TabNavigationStepDefinitions
    {

        private readonly ScenarioContext _context;
        private readonly Browser _browser;
        private DHCW_HomePage _dhcwHomePage;
       // private DHCW_NewsPage _dhcwNewsPage;

        public TabNavigationStepDefinitions(ScenarioContext context, Browser browser)
        {
            _context = context;
            _browser = browser;
            _dhcwHomePage = new DHCW_HomePage(_browser);
            //_dhcwNewsPage = new DHCW_NewsPage(_browser);
        }

        [Given(@"I am on the TODO Website")]
        public void IAmOnTheDHCWWebsite()
        {
            var url = $"{PageUrl.HOME_PAGE}";
            _dhcwHomePage = _browser.NavigateTo<DHCW_HomePage>(url);
        }

        [When(@"I enter my email ""(.*)"" using locator ""(.*)""")]
        public void WhenIEnterMyEmailWithLocator(string email, string locatorName)
        {
            var emailInput = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(LocatorFactory.GetLocator(locatorName))
            );
            emailInput.SendKeys(email);
          //  ExtentReportHelper.Test.Pass("Step description or success message");

        }


        [When(@"I enter my email ""(.*)"" and click submit")]
        public void WhenIEnterMyEmailAndClickSubmit(string email)
        {
            var emailInput = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='email'][placeholder='Email']"))
            );
            emailInput.SendKeys(email);
           
        }

        [When(@"I enter my password ""(.*)"" and submit")]
        public void WhenIEnterMyPasswordAndSubmit(string password)
        {
            var passwordInput = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='password'][placeholder='Password']"))
            );
            passwordInput.SendKeys(password);

            // Assuming the same submit button appears after password entry
            var submitButton = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementToBeClickable(By.XPath("//button[text()='Login']"))
            );
            submitButton.Click();

            // Wait for login to complete and home page to load
            _browser.DriverWaitInstance().Until(driver =>
                driver.Url.Contains("home") || driver.FindElements(By.ClassName("home-page-indicator")).Count > 0);
        }

        [When(@"I click on the ""(.*)"" button")]
        public void WhenIClickOnTheButton(string buttonText)
        {
            var button = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//button[normalize-space()='{buttonText}']"))
            );
            button.Click();
        }

        [When(@"I enter ""(.*)"" in the title field")]
        public void WhenIEnterInTheTitleField(string title)
        {
            var titleField = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.CssSelector("input.title-input[type='text']"))
            );
            titleField.Clear();
            titleField.SendKeys(title);
        }

        [When(@"I enter ""(.*)"" in the description field")]
        public void WhenIEnterInTheDescriptionField(string description)
        {
            var descField = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.CssSelector("textarea[type='text'][placeholder*='Description']"))
            );
            descField.Clear();
            descField.SendKeys(description);
        }

        [When(@"I select ""(.*)"" priority from the dropdown")]
        public void WhenISelectPriorityFromTheDropdown(string priority)
        {
            // Click to open dropdown
            var dropdown = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementToBeClickable(
                    By.CssSelector("div.MuiSelect-root[role='button']"))
            );
            dropdown.Click();

            // Select the priority option
            var priorityOption = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//li[contains(@class,'MuiMenuItem-root') and normalize-space()='{priority}']"))
            );
            priorityOption.Click();
        }

        [Then(@"I click on the ""(.*)"" button")]
        public void ThenIClickOnTheButton(string buttonText)
        {
            var button = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//button[contains(@class,'btnNew') and normalize-space()='{buttonText}']"))
            );
            button.Click();
        }

        [Then(@"the new task should be created successfully")]
        public void ThenTheNewTaskShouldBeCreatedSuccessfully()
        {
            // Verify task appears in the list
            var taskTitle = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.XPath("//div[contains(@class,'task-item')]//*[contains(text(),'automation task')]"))
            );

            // Or verify success message appears
            var successMessage = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.XPath("//div[contains(@class,'alert-success') and contains(text(),'Task created')]"))
            );
        }

        [Given(@"there exists a task named ""(.*)""")]
        public void GivenThereExistsATaskNamed(string taskName)
        {
            var taskElement = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.XPath($"//div[contains(@class,'title')]//h2[normalize-space()='{taskName}']"))
            );
        }

        [When(@"I delete the task named ""(.*)""")]
        public void WhenIDeleteTheTaskNamed(string taskName)
        {
            // Locate the task item first
            var taskElement = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.XPath($"//div[contains(@class,'title')]//h2[normalize-space()='{taskName}']/ancestor::div[contains(@class,'task-item')]"))
            );

            // Find and click the delete button within this task item
            var deleteButton = taskElement.FindElement(
                By.CssSelector("svg.delete, [title='delete']")
            );
            deleteButton.Click();

            // Handle confirmation dialog if appears
            try
            {
                var confirmButton = _browser.DriverWaitInstance(5).Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.XPath("//button[contains(text(),'Confirm') or contains(text(),'Delete')]"))
                );
                confirmButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // No confirmation dialog appeared
            }
        }

        [Then(@"the task should be removed from the list")]
        public void ThenTheTaskShouldBeRemovedFromTheList()
        {
            _browser.DriverWaitInstance().Until(
                ExpectedConditions.InvisibilityOfElementLocated(
                    By.XPath("//div[contains(@class,'title')]//h2[normalize-space()='automation']"))
            );
        }

        [Then(@"I should see error message ""(.*)""")]
        public void ThenIShouldSeeErrorMessage(string expectedErrorMessage)
        {
            var errorElement = _browser.DriverWaitInstance().Until(
                ExpectedConditions.ElementIsVisible(
                    By.CssSelector("p.errorz"))
            );

            Assert.Equals(expectedErrorMessage, errorElement.Text.Trim());
        }

        private RestClient _client;
        private RestRequest _request;
        private RestResponse _response;
        private JObject _responseData;
        private JObject _requestBody;
        private string _userId;

        private HttpResponseMessage _responseHTTP;

        [Given(@"I send a GET request to ""(.*)""")]
        public void GivenISendAGETRequestTo(string url)
        {
            _client = new RestClient(url);
            _request = new RestRequest();
            _response = _client.Get(_request);
            _responseData = JObject.Parse(_response.Content);
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            Assert.AreEqual(expectedStatusCode, (int)_response.StatusCode);
        }

        [Then(@"the response should contain a list of users")]
        public void ThenTheResponseShouldContainAListOfUsers()
        {
            var users = _responseData["data"];
            Assert.IsNotNull(users);
            Assert.IsTrue(users.HasValues);
        }

        [Then(@"each user should have id, email, first_name, last_name and avatar fields")]
        public void ThenEachUserShouldHaveRequiredFields()
        {
            var users = _responseData["data"];

            foreach (var user in users)
            {
                Assert.IsNotNull(user["id"]);
                Assert.IsNotNull(user["email"]);
                Assert.IsFalse(string.IsNullOrEmpty(user["email"].ToString()));
                Assert.IsNotNull(user["first_name"]);
                Assert.IsNotNull(user["last_name"]);

                var avatarUrl = user["avatar"].ToString();
                Assert.IsFalse(string.IsNullOrEmpty(avatarUrl));
                Assert.IsTrue(Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute));
            }
        }

        [Then(@"the response should include pagination information")]
        public void ThenTheResponseShouldIncludePaginationInformation()
        {
            Assert.IsNotNull(_responseData["page"]);
            Assert.IsNotNull(_responseData["per_page"]);
            Assert.IsNotNull(_responseData["total"]);
            Assert.IsNotNull(_responseData["total_pages"]);
        }

        [Given(@"I prepare a new user with name ""(.*)"" and job ""(.*)""")]
        public void GivenIPrepareANewUserWithNameAndJob(string name, string job)
        {
            _requestBody = new JObject
            {
                ["name"] = name,
                ["job"] = job
            };
        }

        [When(@"I send a POST request to ""(.*)""")]
        public void WhenISendAPOSTRequestTo(string url)
        {
            _client = new RestClient(url);
            _request = new RestRequest();

            // Add headers
            _request.AddHeader("x-api-key", "reqres-free-v1");
            _request.AddHeader("Content-Type", "application/json");

            // Add JSON body
            _request.AddJsonBody(_requestBody.ToString());

            _response = _client.Post(_request);
            _responseData = JObject.Parse(_response.Content);
        }


        [Then(@"the response should contain the created user details")]
        public void ThenTheResponseShouldContainTheCreatedUserDetails()
        {
            Assert.IsNotNull(_responseData["id"]);
            Assert.IsNotNull(_responseData["createdAt"]);
        }

        [Then(@"the response should match the request data")]
        public void ThenTheResponseShouldMatchTheRequestData()
        {
            Assert.AreEqual(_requestBody["name"], _responseData["name"]);
            Assert.AreEqual(_requestBody["job"], _responseData["job"]);
        }

        [When(@"I send a PUT request to ""(.*)""")]
        public void WhenISendAPUTRequestTo(string url)
        {
            _client = new RestClient(url); // RestClient instead of HttpClient
            _request = new RestRequest();

            // Add headers
            _request.AddHeader("x-api-key", "reqres-free-v1");
            _request.AddHeader("Content-Type", "application/json");

            // Add JSON body
            _request.AddJsonBody(_requestBody.ToString());

            // Execute PUT request
            _response = _client.Put(_request);
            _responseData = JObject.Parse(_response.Content);
        }

        [Given(@"I prepare user update data with name ""(.*)"" and job ""(.*)""")]
        public void GivenIPrepareUserUpdateDataWithNameAndJob(string name, string job)
        {
            _requestBody = new JObject
            {
                ["name"] = name,
                ["job"] = job
            };
        }

        [Then(@"the response should contain the updated timestamp")]
        public void ThenTheResponseShouldContainTheUpdatedTimestamp()
        {
            Assert.IsNotNull(_responseData["updatedAt"]);
            Assert.IsFalse(string.IsNullOrEmpty(_responseData["updatedAt"].ToString()));
        }

        [Given(@"I have a valid user ID ""(.*)""")]
        public void GivenIHaveAValidUserID(string userId)
        {
            _userId = userId;
        }

        [When(@"I send a DELETE request to ""(.*)""")]
        public void WhenISendADELETERequestTo(string url)
        {
            _client = new RestClient(url);
            _request = new RestRequest();
            _request.AddHeader("x-api-key", "reqres-free-v1");

            _response = _client.Delete(_request);
        }

        [Then(@"the response should be empty")]
        public void ThenTheResponseShouldBeEmpty()
        {
            Assert.IsEmpty(_response.Content);
        }
    }

    [Binding]
    public class ScreenshotHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly Browser _browser;

        public ScreenshotHooks(ScenarioContext scenarioContext, Browser browser)
        {
            _scenarioContext = scenarioContext;
            _browser = browser;
        }

        [AfterScenario]
        public void TakeScreenshotOnFailure()
        {
            if (_scenarioContext.TestError != null)
            {
                try
                {
                    var screenshotsDir = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                    Directory.CreateDirectory(screenshotsDir);

                    var fileName = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var filePath = Path.Combine(screenshotsDir, fileName);

                    var screenshot = ((ITakesScreenshot)_browser.DriverInstance()).GetScreenshot();
                    screenshot.SaveAsFile(filePath);
                }
                catch (Exception ex)
                {
                    // Optionally log screenshot failure
                }
            }
        }
    }
}