using System;
using System.Collections.Generic;

namespace Flow.Connectors.Twitter
{
    public interface ITweet
    {
        IEnumerable<TweetSharp.TwitterStatus> GetLatestTwitterForWorkflow(out DateTime lastTweetOn);
        void SetLastTweetData(DateTime last);
    }
}
