using OpenQA.Selenium;

namespace SpecFlowProjectTest.Support
{
    public class Interceptor : IDisposable
    {
        private readonly INetwork _interceptor;
        public Interceptor(IWebDriver driver)
        {
            _interceptor = driver.Manage().Network;
        }

        public void StartMonitoring()
        {
            _interceptor.StartMonitoring().GetAwaiter().GetResult();
        }

        public void StopMonitoring()
        {
            _interceptor.ClearResponseHandlers();
            _interceptor.StopMonitoring().GetAwaiter().GetResult();
        }

        public void Intercept(int statusCode, string partialUrl, string responseBody = "{}")
        {
            var handler = new NetworkResponseHandler
            {
                ResponseMatcher = req =>
                {
                    if (partialUrl.ToLower().StartsWith("ends-with::"))
                    {
                        var endsWithUrl = partialUrl.Replace("ends-with::", "");
                        return req.Url.EndsWith(endsWithUrl);

                    }
                    if (partialUrl.ToLower().StartsWith("contains::"))
                    {
                        var containsUrl = partialUrl.Replace("contains::", "");
                        return req.Url.Contains(containsUrl);
                    }
                    else
                    {
                        return req.Url.Contains(partialUrl);
                    }
                },
                ResponseTransformer = http =>
                {
                    http.StatusCode = statusCode;
                    http.Body = responseBody;
                    return http;
                }
            };
            _interceptor.AddResponseHandler(handler);
            _interceptor.StartMonitoring().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _interceptor.StopMonitoring();
            _interceptor.ClearRequestHandlers();
            GC.SuppressFinalize(this);
        }
    }
}
