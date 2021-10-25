using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace TwitterStreamAnalyzer.Services.Tests
{
    [TestClass()]
    public class TwitterServiceCallTests
    {
        private HttpClient _http;

        [TestMethod()]
        public void GetSampleStreamTest()
        {
            _http = new HttpClient();
            string BEARER_TOKEN = ConfigurationManager.AppSettings["BEARER_TOKEN"];
            string requestUri = ConfigurationManager.AppSettings["requestUri"];
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BEARER_TOKEN);
            var stream = _http.GetStreamAsync(requestUri).Result;

            if (stream == null)
                Assert.Fail();
        }
    }
}