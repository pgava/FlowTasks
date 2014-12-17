using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Tasks.Data;
using Flow.Tasks.Data.Core;
using Flow.Library;
using Flow.Tasks.Data.Infrastructure;
using Flow.Tasks.Contract.Message;
using System.IO;

namespace Flow.Tasks.Test
{
    [TestClass]
    public sealed class DBTest : IDisposable
    {

        private FlowTasksUnitOfWork uow;        

        private TestContext testContextInstance;

        #region IDisposable
        public void Dispose()
        {
            if (uow != null)
            {
                var disp = uow as FlowTasksUnitOfWork;
                disp.Dispose();
            }

        }
        #endregion

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

        [TestInitialize]
        public void CreateContext()
        {
            uow = new FlowTasksUnitOfWork();
        }

        [TestCleanup()]
        public void MyTestCleanup() 
        {
            uow.Commit();
            uow.Dispose();
        }

        [TestMethod]
        [Ignore]
        public void CreateDb()
        {
            TraceEvent teDebug = CreateEvent(TraceEventType.Debug, "Debug");
            TraceEvent teActivity = CreateEvent(TraceEventType.Activity, "Activity");
            TraceEvent teInfo = CreateEvent(TraceEventType.Info, "Info");
            TraceEvent teError = CreateEvent(TraceEventType.Error, "Error");
            uow.TraceEvents.Insert(teDebug);
            uow.TraceEvents.Insert(teActivity);
            uow.TraceEvents.Insert(teInfo);
            uow.TraceEvents.Insert(teError);

            WorkflowStatus wsInProgress = CreateStatus(WorkflowStatusType.InProgress, "InProgress");
            WorkflowStatus wsCompleted = CreateStatus(WorkflowStatusType.Completed, "Completed");
            WorkflowStatus wsAborted = CreateStatus(WorkflowStatusType.Aborted, "Aborted");
            WorkflowStatus wsTerminated = CreateStatus(WorkflowStatusType.Terminated, "Terminated");

            uow.WorkflowStatuses.Insert(wsInProgress);
            uow.WorkflowStatuses.Insert(wsCompleted);
            uow.WorkflowStatuses.Insert(wsAborted);
            uow.WorkflowStatuses.Insert(wsTerminated);

            SketchStatus skSaved = CreateSketchStatus("Saved", "Sketch Saved");
            SketchStatus skDeployedDev = CreateSketchStatus("DeployedDev", "Deployed to dev");
            SketchStatus skDeployedProd = CreateSketchStatus("DeployedProd", "Deployed to prod");
            SketchStatus skSentToSketch = CreateSketchStatus("SentToSketch", "Sent To Sketch");
            SketchStatus skAborted = CreateSketchStatus("Aborted", "Abort workflow deployment");

            uow.SketchStatuses.Insert(skSaved);
            uow.SketchStatuses.Insert(skDeployedDev);
            uow.SketchStatuses.Insert(skDeployedProd);
            uow.SketchStatuses.Insert(skSentToSketch);
            uow.SketchStatuses.Insert(skAborted);

            WorkflowCode wc1 = CreateWorkflowCode("SampleWf1", "this is a sample wf code for testing (1)");
            uow.WorkflowCodes.Insert(wc1);
            WorkflowConfiguration wfc1 = CreateWorkflowConfiguration(wc1, "BasicHttpBinding_FlowTasks", "", "");
            uow.WorkflowConfigurations.Insert(wfc1);

            WorkflowCode wc2 = CreateWorkflowCode("SampleWf2", "this is a sample wf code for testing (2)");
            uow.WorkflowCodes.Insert(wc2);
            WorkflowConfiguration wfc2 = CreateWorkflowConfiguration(wc2, "BasicHttpBinding_IFlowTasksOperations2", "http://localhost/Flow.Tasks.Workflows/SampleWf2.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc2);

            WorkflowCode wc3 = CreateWorkflowCode("SampleWf3", "this is a sample wf code for testing (3)");
            uow.WorkflowCodes.Insert(wc3);
            WorkflowConfiguration wfc3 = CreateWorkflowConfiguration(wc3, "BasicHttpBinding_IFlowTasksOperations3", "http://localhost/Flow.Tasks.Workflows/SampleWf3.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc3);

            WorkflowCode wc4 = CreateWorkflowCode("SampleWf4", "this is a sample wf code for testing (4)");
            uow.WorkflowCodes.Insert(wc4);
            WorkflowConfiguration wfc4 = CreateWorkflowConfiguration(wc4, "BasicHttpBinding_IFlowTasksOperations4", "http://localhost/Flow.Tasks.Workflows/SampleWf4.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc4);

            WorkflowCode wc5 = CreateWorkflowCode("SampleWf5", "this is a sample wf code for testing (5)");
            uow.WorkflowCodes.Insert(wc5);
            WorkflowConfiguration wfc5 = CreateWorkflowConfiguration(wc5, "BasicHttpBinding_IFlowTasksOperations5", "http://localhost/Flow.Tasks.Workflows/SampleWf5.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc5);

            WorkflowCode wc6 = CreateWorkflowCode("SampleWf6", "this is a sample wf code for testing (6)");
            uow.WorkflowCodes.Insert(wc6);
            WorkflowConfiguration wfc6 = CreateWorkflowConfiguration(wc6, "BasicHttpBinding_IFlowTasksOperations6", "http://localhost/Flow.Tasks.Workflows/SampleWf6.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc6);

            WorkflowCode wc7 = CreateWorkflowCode("SampleWf7", "this is a sample wf code for testing (7)");
            uow.WorkflowCodes.Insert(wc7);
            WorkflowConfiguration wfc7 = CreateWorkflowConfiguration(wc7, "BasicHttpBinding_IFlowTasksOperations7", "http://localhost/Flow.Tasks.Workflows/SampleWf7.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc7);

            WorkflowCode wc8 = CreateWorkflowCode("SampleWf8", "this is a sample wf code for testing (8)");
            uow.WorkflowCodes.Insert(wc8);
            WorkflowConfiguration wfc8 = CreateWorkflowConfiguration(wc8, "BasicHttpBinding_IFlowTasksOperations8", "http://localhost/ServiceWorkflowsVB/SampleWf8.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc8);
            
            WorkflowCode wc9 = CreateWorkflowCode("SampleWf9", "this is a sample wf code for testing (9)");
            uow.WorkflowCodes.Insert(wc9);
            WorkflowConfiguration wfc9 = CreateWorkflowConfiguration(wc9, "BasicHttpBinding_IFlowTasksOperations9", "http://localhost/ServiceWorkflowsVB/SampleWf9.xamlx", "BasicHttpBinding_FlowTasks");
            uow.WorkflowConfigurations.Insert(wfc9);

            // Topic
            TopicStatus topicNew = CreateTopicStatus("New", "New topic message");
            TopicStatus topicRead = CreateTopicStatus("Read", "Read topic message");
            uow.TopicStatuses.Insert(topicNew);
            uow.TopicStatuses.Insert(topicRead);
        }

        private TopicAttachment CreateTopicAttachment(string file, Guid oid, TopicMessage m)
        {
            return new TopicAttachment { FileName = file, OidDocument = oid, TopicMessage = m };
        }

        private TopicUser CreateTopicUser(string user, TopicMessage msg, TopicStatus topicRead)
        {
            return new TopicUser { User = user, TopicMessage = msg, TopicStatus = topicRead };
        }

        private TopicStatus CreateTopicStatus(string status, string description)
        {
            return new TopicStatus { Status = status, Description = description };
        }

        private TopicMessage CreateMessage(string msg, string from, string to, DateTime when, Boolean isTopic, Topic t)
        {
            return new TopicMessage { Message = msg, From = from, To = to, When = when, IsTopic = isTopic,  Topic = t };
        }

        private Topic CreateTopic(string title)
        {
            return new Topic { Title = title };
        }

        private TaskUserHandOver CreateTaskUserHandOver(TaskDefinition taskDefinition, string user)
        {
            return new TaskUserHandOver
            {
                TaskDefinition = taskDefinition,
                InUse = false,
                User = user
            };
        }

        private TaskUser CreateTaskUser(TaskDefinition taskDefinition, string user)
        {
            return new TaskUser
            {
                TaskDefinition = taskDefinition,
                InUse = false,
                User = user
            };
        }

        private TaskDefinition CreateTaskDefinition(WorkflowDefinition workflowDefinition, string title, string desc, string uiCode)
        {
            return new TaskDefinition
            {
                Description = desc,
                Title = title,
                UiCode = uiCode,
                WorkflowDefinition = workflowDefinition,
                TaskOid = Guid.NewGuid()
            };
        }

        private TaskConfiguration CreateTaskConfiguration(WorkflowCode workflowCode, string taskCode)
        {
            return new TaskConfiguration
            {
                Description = "New Description",
                Title = "New titile",
                TaskCode = taskCode,
                WorkflowCode = workflowCode
            };
        }

        private WorkflowTrace CreateWorkflowTrace(TraceEvent traceEvent, WorkflowDefinition workflowDefinition,
            string message, DateTime when, string user)
        {
            return new WorkflowTrace
            {
                TraceEvent = traceEvent,
                WorkflowDefinition = workflowDefinition,
                Message = message,
                User = user,
                When = when
            };
        }

        private WorkflowDefinition CreateWorkflowDefinition(WorkflowCode workflowCode, WorkflowStatus workflowStatus)
        {
            return new WorkflowDefinition
            {
                WorkflowOid = Guid.NewGuid(),
                WorkflowCode = workflowCode,
                StartedOn = DateTime.Now,
                WorkflowStatus = workflowStatus
            };
        }

        private WorkflowProperty CreateWorkflowProperty(Property property, WorkflowCode workflowCode)
        {
            return new WorkflowProperty
            {
                Property = property,
                WorkflowCode = workflowCode
            };
        }

        private Property CreateProperty(string name, string value)
        {
            return new Property
            {
                Name = name,
                Value = value,
                Type = "S"
            };
        }

        private TraceEvent CreateEvent(TraceEventType type, string desc)
        {
            return new TraceEvent
            {
                Type = type.ToString(),                
                Description = desc
            };
        }

        private WorkflowStatus CreateStatus(WorkflowStatusType status, string desc)
        {
            return new WorkflowStatus
            {
                Status = status.ToString(),                
                Description = desc
            };
        }

        private SketchStatus CreateSketchStatus(string status, string desc)
        {
            return new SketchStatus
            {
                Status = status,                
                Description = desc
            };
        }

        private WorkflowCode CreateWorkflowCode(string code, string desc)
        {
            return new WorkflowCode
            {
                Code = code,
                Description = desc
            };
        }

        private WorkflowConfiguration CreateWorkflowConfiguration(WorkflowCode workflowCode, string serviceEndpoint, string serviceUrl, string bindingConfiguration)
        {
            return new WorkflowConfiguration
            {
                EffectiveDate = DateTime.Now,
                ServiceUrl = serviceUrl,
                ServiceEndpoint = serviceEndpoint,
                BindingConfiguration = bindingConfiguration,
                WorkflowCode = workflowCode
                //ServiceDefinition = LoadWorkflow(workflowCode.Code)
            };
        }

        //private string LoadWorkflow(string code)
        //{
        //    var path = @"..\..\..\Flow.Workflows.Test\ServiceWorkflows\";

        //    var file = Path.Combine(path, code + ".xamlx");
        //    return File.ReadAllText(file);
        //}
    }
}
