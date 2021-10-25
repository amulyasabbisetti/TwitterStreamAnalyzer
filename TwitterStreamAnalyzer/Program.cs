using System;
using TwitterStreamAnalyzer.Business;
using TwitterStreamAnalyzer.Services;

namespace TwitterStreamAnalyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var service = new TwitterService();

                var process = new TwitterDataProcessing(service);
                process.startProcessing();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
