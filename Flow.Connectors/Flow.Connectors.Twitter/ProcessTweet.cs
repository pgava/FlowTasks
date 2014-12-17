using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Connectors.Twitter.Configuration;
using log4net;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;
using Flow.Tasks.Contract;

namespace Flow.Connectors.Twitter
{
    public class ProcessTweet
    {
        // Logger object.
        private static readonly ILog _log =
           LogManager.GetLogger(typeof(ProcessTweet));

        private ITweet _tweet;
        private IFlowTasksService _flowTasksService;

        public ProcessTweet(ITweet tweet, IFlowTasksService flowTasksService)
        {
            _tweet = tweet;
            _flowTasksService = flowTasksService;
        }

        public ProcessTweet(ITweet tweet)
        {
            _tweet = tweet;
        }

        public void DoProcess()
        {
            DateTime last;

            try
            {
                var tweets = _tweet.GetLatestTwitterForWorkflow(out last);

                DateTime lastRun = DateTime.MinValue;
                foreach (var t in tweets)
                {
                    var startWorkflowRequest = new StartWorkflowRequest
                    {
                        Domain = ConfigHelper.WorkflowDomain,
                        WorkflowCode = ConfigHelper.WorkflowCode,
                        WfRuntimeValues = new WfProperty[] 
                        {
                            new WfProperty
                            {
                                Name = ConfigHelper.WorkflowProperty,
                                Type = "S",
                                Value = t.Text
                            }
                        }
                    };

                    StartWorkflowResponse startWorkflowResponse = null;
                    if (_flowTasksService == null)
                    {
                        using (FlowTasksService proxy = new FlowTasksService())
                        {
                            startWorkflowResponse = proxy.StartWorkflow(startWorkflowRequest);
                        }
                    }
                    else
                    {
                        startWorkflowResponse = _flowTasksService.StartWorkflow(startWorkflowRequest);
                    }

                    // check for errors
                    if (string.IsNullOrWhiteSpace(startWorkflowResponse.WorkflowId) ||
                        startWorkflowResponse.WorkflowId == Guid.Empty.ToString())
                    {
                        if (lastRun != DateTime.MinValue)
                        {
                            _tweet.SetLastTweetData(lastRun);
                        }

                        _log.Error("DoProcess: Start workflow failed!");

                        return;
                    }

                    // last tweet processed
                    lastRun = t.CreatedDate;
                }

                _tweet.SetLastTweetData(last);

            }
            catch (Exception e)
            {
                // log the error
                _log.Error("DoProcess: exception. ", e);

            }
        }
    }
}
