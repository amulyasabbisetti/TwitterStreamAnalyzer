using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamAnalyzer.Interfaces;

namespace TwitterStreamAnalyzer.Business
{
    class TwitterDataProcessing : ITweetProcessing
    {
        ITweetService service;
        Dictionary<DateTime, int> twitterData;

        public TwitterDataProcessing(ITweetService service)
        {
            this.service = service;
            this.twitterData = new Dictionary<DateTime, int>();
        }

        public void startProcessing()
        {
            Console.WriteLine("Data collection started at - " + DateTime.Now);

            Task.Factory.StartNew(() => service.GetSampleStream(twitterData));
            ProcessData();
        }

        public void ProcessData()
        {

            while (!twitterData.Any())
                Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["intialWait"])); //adding a wait of 1 minute to collect the intial data
            KeyValuePair<DateTime, int> firstTweet;
            lock (twitterData)
            {
                firstTweet = twitterData.First();
            }

            Console.WriteLine("Total tweets received in the first ping - " + firstTweet.Value);

            DateTime currentTime = firstTweet.Key;
            int iteration = 2;
            int averageTweets = firstTweet.Value;
           
            while (true)
            {
                IEnumerable<KeyValuePair<DateTime, int>> oneMinuteTweets;
                lock (twitterData)
                {
                    oneMinuteTweets = twitterData.Where(item => item.Key > currentTime
                                    && item.Key <= currentTime.AddSeconds(Double.Parse(ConfigurationManager.AppSettings["timeInterval"]))).ToList();                  
                }

                if (oneMinuteTweets.Any())
                {
                    var currentTweet = oneMinuteTweets.Last();
                    currentTime = currentTweet.Key;
                    averageTweets -= averageTweets / iteration;
                    averageTweets += currentTweet.Value / iteration;
                    Console.WriteLine("Total tweets received in the last minute - " + oneMinuteTweets.Sum(item => item.Value));
                    Console.WriteLine("Average tweets received per minute - " + averageTweets);
                    iteration++;
                }
                else
                {
                    currentTime = DelayDateTimeUpdate(currentTime);
                }
            }
        }

        private DateTime DelayDateTimeUpdate(DateTime delayTimer)
        {
            if(twitterData.Any(item => item.Key > delayTimer.AddSeconds(Double.Parse(ConfigurationManager.AppSettings["timeInterval"])) ))
            {
                return twitterData.First(item => item.Key > delayTimer.AddSeconds(Double.Parse(ConfigurationManager.AppSettings["timeInterval"]))).Key.AddSeconds(-1);
            }

            return delayTimer;
        }
    }
}
