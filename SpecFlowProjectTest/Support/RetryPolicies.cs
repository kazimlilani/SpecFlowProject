using OpenQA.Selenium;
using Polly;
using Polly.Retry;

namespace SpecFlowProjectTest.Support
{
    public static class RetryPolicies
    {
        public static RetryPolicy<bool> RetryBooleanOrDriverException(bool conditionToHandle, int retryCount = 10, double secondsBetweenAttempts = 1)
        {
            return Policy
                .HandleResult(conditionToHandle)
                .Or<WebDriverException>()
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        public static RetryPolicy<bool> RetryHandleBoolean(bool conditionToHandle, int retryCount = 30, double secondsBetweenAttempts = 0.5)
        {
            return Policy
                .HandleResult(conditionToHandle)
                .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(secondsBetweenAttempts));
        }

        public static RetryPolicy RetryException(int retryCount = 10)
        {
            return Policy
                       .Handle<WebDriverException>()
                       .Or<NoSuchElementException>()
                       .Or<StaleElementReferenceException>()
                       .Or<ElementNotInteractableException>()
                       .Or<ElementClickInterceptedException>()
                       .Or<Exception>()
                       .WaitAndRetry(retryCount, x => TimeSpan.FromSeconds(1));
        }

    }
}

