using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweet = Flow.Connectors.Twitter;
using Flow.Connectors.Twitter;
using Moq;
using TweetSharp;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;

namespace Twitter.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TwitterTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Read_Tweets()
        {
            var twitter = new tweet.Tweet();
            DateTime last;
            var tweets = twitter.GetLatestTwitterForWorkflow(out last);

        }

        [TestMethod]
        public void Process_Tweets_When_No_Tweets_To_Process()
        {
            var mockTweet = new Mock<ITweet>();
            var mockTask = new Mock<IFlowTasksService>();

            DateTime last = new DateTime(2012, 5, 30);
            List<TwitterStatus> tweets = new List<TwitterStatus>();
            mockTweet.Setup(tweet => tweet.GetLatestTwitterForWorkflow(out last))
                .Returns(tweets);
            mockTweet.Setup(tweet => tweet.SetLastTweetData(last));

            var resp = new StartWorkflowResponse
            {
                WorkflowId = Guid.NewGuid().ToString()
            };
            mockTask.Setup(task => task.StartWorkflow(Moq.It.IsAny<StartWorkflowRequest>()))
                .Returns(resp);

            var sut = new ProcessTweet(mockTweet.Object, mockTask.Object);
            sut.DoProcess();

            mockTweet.Verify(tweet => tweet.SetLastTweetData(new DateTime(2012, 5, 30)));
            mockTask.Verify(task => task.StartWorkflow(Moq.It.IsAny<StartWorkflowRequest>()), Times.Never());
        }

        [TestMethod]
        public void Process_All_Tweets()
        {
            var mockTweet = new Mock<ITweet>();
            var mockTask = new Mock<IFlowTasksService>();

            DateTime last = new DateTime(2012, 5, 22);
            List<TwitterStatus> tweets = new List<TwitterStatus>();
            tweets.Add(new TwitterStatus
                    {
                        CreatedDate = new DateTime(2012, 5, 1),
                        Text = "Tweet 1",
                        
                    });
            tweets.Add(new TwitterStatus
                    {
                        CreatedDate = new DateTime(2012, 5, 2),
                        Text = "Tweet 2",
                        
                    });

            mockTweet.Setup(tweet => tweet.GetLatestTwitterForWorkflow(out last))
                .Returns(tweets);
            mockTweet.Setup(tweet => tweet.SetLastTweetData(last));

            var resp = new StartWorkflowResponse
            {
                WorkflowId = Guid.NewGuid().ToString()
            };
            mockTask.Setup(task => task.StartWorkflow(Moq.It.IsAny<StartWorkflowRequest>()))
                .Returns(resp);

            var sut = new ProcessTweet(mockTweet.Object, mockTask.Object);
            sut.DoProcess();

            mockTweet.Verify(tweet => tweet.SetLastTweetData(new DateTime(2012, 5, 22)));
        }

        [TestMethod]
        public void Process_Only_First_Tweet()
        {
            var mockTweet = new Mock<ITweet>();
            var mockTask = new Mock<IFlowTasksService>();

            DateTime last = new DateTime(2012, 5, 30);
            List<TwitterStatus> tweets = new List<TwitterStatus>();
            tweets.Add(new TwitterStatus
            {
                CreatedDate = new DateTime(2012, 5, 1),
                Text = "Tweet 1",

            });
            tweets.Add(new TwitterStatus
            {
                CreatedDate = new DateTime(2012, 5, 2),
                Text = "Tweet 2",

            });

            mockTweet.Setup(tweet => tweet.GetLatestTwitterForWorkflow(out last))
                .Returns(tweets);
            mockTweet.Setup(tweet => tweet.SetLastTweetData(last));

            var resp = new StartWorkflowResponse
            {
                WorkflowId = Guid.NewGuid().ToString()
            };

            mockTask.Setup(task => task.StartWorkflow(Moq.It.IsAny<StartWorkflowRequest>()))
                .Returns(() => resp)
                .Callback(() => resp = new StartWorkflowResponse { WorkflowId = Guid.Empty.ToString() } );

            var sut = new ProcessTweet(mockTweet.Object, mockTask.Object);
            sut.DoProcess();

            mockTweet.Verify(tweet => tweet.SetLastTweetData(new DateTime(2012, 5, 1)));
        }

    }
}
