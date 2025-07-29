using OpenQA.Selenium;

namespace SpecFlowProjectTest.Support
{
    public static class WebElementExtensions
    {
        public static void ForceClick(this IWebElement element, IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver)
         .ExecuteScript("arguments[0].click();", element);
        }

        public static bool IsStale(this IWebElement element)
        {
            try
            {
                var willThrowAnExceptionWhenStale = element.Enabled;
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }

    }
}

