using System;
using System.Collections.Generic;

namespace TwitterStreamAnalyzer.Interfaces
{
    public interface ITweetService
    {
        void GetSampleStream(Dictionary<DateTime, int> twitterData);
    }
}
