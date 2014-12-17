using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tweet = TweetSharp;
using Flow.Connectors.Twitter.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Flow.Connectors.Twitter
{
    /// <summary>
    /// Tweet
    /// </summary>
    public class Tweet : ITweet
    {
        private readonly string _consumerKey = ConfigHelper.ConsumerKey;
        private readonly string _consumerSecret = ConfigHelper.ConsumerSecret;
        private readonly string _accessToken = ConfigHelper.AccessToken;
        private readonly string _accessTokenSecret = ConfigHelper.AccessTokenSecret;
        private readonly tweet.TwitterService _twitterService;
        private TweetData _twitterData;

        public Tweet()
        {
            _twitterService = GetAuthenticatedService();
            _twitterData = GetTwitterData();
        }

        /// <summary>
        /// Get Authenticated Service
        /// </summary>
        /// <returns>TwitterService</returns>
        private tweet.TwitterService GetAuthenticatedService()
        {
            var service = new tweet.TwitterService(_consumerKey, _consumerSecret);
            service.AuthenticateWith(_accessToken, _accessTokenSecret);

            return service;
        }

        /// <summary>
        /// Get Twitter Data
        /// </summary>
        /// <returns>TweetData</returns>
        private TweetData GetTwitterData()
        {
            XmlDocument pwdDoc = new XmlDocument();
            pwdDoc.Load(ConfigHelper.PathTwitterData);

            using (var reader = new XmlNodeReader(pwdDoc))            
            {
                var serial = new XmlSerializer(typeof(TweetData));
                return serial.Deserialize(reader) as TweetData;
            }
        }

        /// <summary>
        /// Set Last Tweet Data
        /// </summary>
        /// <param name="last">Last time run</param>
        public void SetLastTweetData(DateTime last)
        {
            _twitterData.LastTwitter = last;

            using (var writer = new StreamWriter(ConfigHelper.PathTwitterData))
            {
                var serial = new XmlSerializer(typeof(TweetData));
                serial.Serialize(writer, _twitterData);
            }
        }

        /// <summary>
        /// Get Latest Twitter For Workflow
        /// </summary>
        /// <param name="lastTweetOn">lastTweetOn</param>
        /// <returns>TwitterStatus</returns>
        public IEnumerable<tweet.TwitterStatus> GetLatestTwitterForWorkflow(out DateTime lastTweetOn)
        {
            var tweets = new List<tweet.TwitterStatus>();
            var page = 1;
            lastTweetOn = DateTime.MinValue;
            while (!GetLatestTwitterForWorkflow(tweets, page++, 2, ref lastTweetOn)) ;

            return tweets.OrderBy(t => t.CreatedDate);
        }

        /// <summary>
        /// Get Latest Twitter For Workflow
        /// </summary>
        /// <param name="tweets">Tweets</param>
        /// <param name="page">Page</param>
        /// <param name="count">Count</param>
        /// <param name="last">Last</param>
        /// <returns>True if all tweets processed</returns>
        private bool GetLatestTwitterForWorkflow(IList<tweet.TwitterStatus> tweets, int page, int count, ref DateTime last)
        {
            // ListTweetsOnSpecifiedUserTimeline is not defined anymore...
            //var ts = _twitterService.ListTweetsOnSpecifiedUserTimeline(ConfigHelper.TwitterUser, page, count);

            var options = new tweet.ListTweetsOnUserTimelineOptions { ScreenName = ConfigHelper.TwitterUser };
            var ts = _twitterService.ListTweetsOnUserTimeline(options);

            bool isAll = true;
            if (ts == null) return true;

            foreach (var t in ts)
            {
                last = (last < t.CreatedDate) ? t.CreatedDate : last;
                if (t.CreatedDate <= _twitterData.LastTwitter)
                {
                    isAll = true;
                    break;
                }
                else isAll = false;

                if (_twitterData.LastTwitter < t.CreatedDate && 
                    t.Text.Contains(ConfigHelper.TwitterPattern))
                {
                    tweets.Add(t);
                }
            }

            return isAll;
        }
    }
}

