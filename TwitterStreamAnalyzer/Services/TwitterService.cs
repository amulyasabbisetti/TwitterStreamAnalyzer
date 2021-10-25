using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using TwitterStreamAnalyzer.Interfaces;

namespace TwitterStreamAnalyzer.Services
{
    public class TwitterService : ITweetService
    {
        HttpClient _http;

        public TwitterService()
        {
        }

        public void GetSampleStream(Dictionary<DateTime, int> twitterData)
        {
            _http = new HttpClient();

            string BEARER_TOKEN = ConfigurationManager.AppSettings["BEARER_TOKEN"];
            string requestUri = ConfigurationManager.AppSettings["requestUri"];
            var timeInterval = ConfigurationManager.AppSettings["timeInterval"];

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BEARER_TOKEN);

            _http.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            var stream = _http.GetStreamAsync(requestUri).Result;

            using (var reader = new StreamReader(stream))
            {
                List<string> oneMinute = new List<string>();
                int count = 0;
                Stopwatch s = new Stopwatch();

                while (!reader.EndOfStream)
                {
                    if (!s.IsRunning)
                        s.Start();

                    var currentLine = reader.ReadLine();
                    count++;
                    if (s.ElapsedMilliseconds >= Convert.ToInt32(timeInterval))
                    {
                        s.Restart();
                        lock (twitterData)
                        {
                            twitterData.Add(GetDateTime(), count);
                        }
                        count = 0;
                    }
                }
            }
        }

        private DateTime GetDateTime()
        {
            return DateTime.Parse(
                DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"),
                System.Globalization.CultureInfo.CurrentCulture
              );
        }
    }
}
