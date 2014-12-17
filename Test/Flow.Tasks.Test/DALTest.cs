using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Flow.Tasks.Data;
using Flow.Library;
using Flow.Tasks.Data.Infrastructure;
using Flow.Tasks.Data.DAL;
using Flow.Tasks.Data.Core;
using Flow.Users.Data;
using Flow.Users.Data.Infrastructure;
using Flow.Users.Data.DAL;
using Flow.Tasks.Contract.Message;
using Moq;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;
using System.IO;

namespace Flow.Tasks.Test
{
    [TestClass]
    public class DALTest
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

        [TestInitialize]
        public void CreateContext()
        {

            WorkflowCode wf = new WorkflowCode
            {
                Code = WORKFLOW_CODE,
                Description = "This is a simple workflow"
            };

            WorkflowConfiguration wfc1 = new WorkflowConfiguration
            {
                EffectiveDate = new DateTime(2000, 1, 1),
                ExpiryDate = new DateTime(2011, 1, 1),
                ServiceUrl = "http://localhost/Flow.Tasks.Workflows/SampleWf1.xamlx",
                ServiceEndpoint = "BasicHttpBinding_IFlowTasksOperations",
                WorkflowCode = wf
            };

            WorkflowConfiguration wfc2 = new WorkflowConfiguration
            {
                EffectiveDate = new DateTime(2011, 1, 2),
                ServiceUrl = "http://localhost/Flow.Tasks.Workflows/SampleWf1.xamlx",
                ServiceEndpoint = "BasicHttpBinding_IFlowTasksOperations",
                WorkflowCode = wf
            };

            Property p1 = new Property
            {
                Name = "Test>>GlobalProp",
                Type = "S",
                Value = "GlobalPropVal"
            };

            WorkflowProperty wfp1 = new WorkflowProperty
            {
                Property = p1,
                WorkflowCode = wf
            };

            SketchConfiguration sc = new SketchConfiguration
            {
                ChangedBy = "cgrant",
                LastSavedOn = DateTime.Now.AddMonths(-3),
                Name = "sketch",
                XamlxOid = Guid.Empty
            };
            var saved = SketchStatusType.Saved.ToString();

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var sketchStatus = uowTasks.SketchStatuses.FirstOrDefault(s => s.Status == saved);

                uowTasks.WorkflowConfigurations.Insert(wfc1);
                uowTasks.WorkflowConfigurations.Insert(wfc2);
                uowTasks.WorkflowProperties.Insert(wfp1);

                sc.SketchStatus = sketchStatus;
                uowTasks.SketchConfigurations.Insert(sc);
                uowTasks.Commit();
            }
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wcode = (from c in uowTasks.WorkflowCodes.AsQueryable()
                             where c.Code == WORKFLOW_CODE
                             select c).FirstOrDefault();
                uowTasks.WorkflowCodes.Delete(wcode);

                var wcode1 = (from c in uowTasks.WorkflowCodes.AsQueryable()
                             where c.Code == WORKFLOW_CODE1
                             select c).FirstOrDefault();
                if (wcode1 != null) uowTasks.WorkflowCodes.Delete(wcode1);

                var props = from p in uowTasks.Properties.AsQueryable()
                            where p.Name.StartsWith("Test>>")
                            select p;
                foreach (var p in props)
                {
                    uowTasks.Properties.Delete(p);
                }

                foreach (var s in uowTasks.SketchConfigurations.AsQueryable())
                {
                    uowTasks.SketchConfigurations.Delete(s);
                }

                var topics = from t in uowTasks.Topics.AsQueryable()
                             where t.Title.StartsWith("Test>>")
                             select t;
                foreach (var t in topics)
                {
                    uowTasks.Topics.Delete(t);
                }

                uowTasks.Commit();

            }
        }

        const string WORKFLOW_CODE = "WF_CODE_TEST";
        const string WORKFLOW_CODE1 = "WF_CODE_TEST1";

        [TestMethod]
        public void Should_Be_Able_To_Create_A_Workflow()
        {
            var oid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>prop1", Value = "val1"},
                    new PropertyInfo{Name = "Test>>prop2", Value = "val2"}
                });

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wdefs = (from wd in uowTasks.WorkflowDefinitions.AsQueryable()
                             where wd.WorkflowOid == oid
                             select wd).ToList();
                var wips = (from wp in uowTasks.WorkflowInParameters.AsQueryable()
                            .Include("Property")
                            where wp.WorkflowDefinition.WorkflowOid == oid
                            select wp).ToList();
                Assert.AreEqual(1, wdefs.Count());
                Assert.AreEqual(3, wips.Count());
                Assert.AreEqual(wips[0].Property.Name, "Test>>GlobalProp");
                Assert.AreEqual(wips[1].Property.Name, "Test>>prop1");
                Assert.AreEqual(wips[2].Property.Name, "Test>>prop2");
            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Add_A_Workflow()
        {
            AddWorkflow(WORKFLOW_CODE1, "url", "config", null);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfc = (from wc in uowTasks.WorkflowCodes.AsQueryable()
                           where wc.Code == WORKFLOW_CODE1
                           select wc).ToList();
                var wfcfg = (from wc in uowTasks.WorkflowConfigurations.AsQueryable()
                           where wc.WorkflowCode.Code == WORKFLOW_CODE1
                           select wc).ToList();

                Assert.IsNotNull(wfc);
                Assert.IsNotNull(wfcfg);
                Assert.AreEqual(1, wfcfg.Count());
                Assert.AreEqual("url", wfcfg[0].ServiceUrl);
                Assert.AreEqual("config", wfcfg[0].BindingConfiguration);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Add_An_Existing_Workflow()
        {
            AddWorkflow(WORKFLOW_CODE, "url", "config", null);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfc = (from wc in uowTasks.WorkflowCodes.AsQueryable()
                           where wc.Code == WORKFLOW_CODE
                           select wc).ToList();
                var wfcfg = (from wc in uowTasks.WorkflowConfigurations.AsQueryable()
                             where wc.WorkflowCode.Code == WORKFLOW_CODE && wc.ExpiryDate == null
                             select wc).ToList();

                Assert.IsNotNull(wfc);
                Assert.IsNotNull(wfcfg);
                Assert.AreEqual(1, wfcfg.Count());
                Assert.AreEqual("url", wfcfg[0].ServiceUrl);
                Assert.AreEqual("config", wfcfg[0].BindingConfiguration);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Terminate_A_Workflow()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                                    {
                                        new PropertyInfo{Name = "Test>>Wfprop1", Value = "val1"}
                                    });

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}"
                },
                new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>TaskProp1", Value = "val1"}
                }
            );

            CompleteWorkflow(workflowOid, WorkflowStatusType.Terminated);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var task = (from td in uowTasks.TaskDefinitions.AsQueryable()
                            where td.TaskOid == taskOid
                            select td).FirstOrDefault();

                var workflow = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                                .Include("WorkflowStatus")
                                where w.WorkflowOid == workflowOid
                                select w).FirstOrDefault();
                Assert.IsNull(task);
                Assert.AreEqual(workflow.WorkflowStatus.Status, WorkflowStatusType.Terminated.ToString());
            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Create_A_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>title1", Value = "the title"},
                    new PropertyInfo{Name = "Test>>desc1", Value = "the desc"}
                });

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    Title = "This is {p.Test>>title1}",
                    Description = "This is {p.Test>>desc1}",
                    TaskCode = "Code1",
                    UiCode = "UICode1",
                    DefaultResult = "OK",
                    AssignedToUsers = "{r.Dev}"
                },
                new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>task1", Value = "task1"},
                    new PropertyInfo{Name = "Test>>task2", Value = "task2"}
                }
            );

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var task = (from td in uowTasks.TaskDefinitions.AsQueryable()
                            where td.TaskOid == taskOid
                            select td).FirstOrDefault();
                var users = (from tu in uowTasks.TaskUsers.AsQueryable()
                             where tu.TaskDefinitionId == task.TaskDefinitionId
                             select tu).ToList();
                var parameters = (from tp in uowTasks.TaskInParameters.AsQueryable()
                                  .Include("Property")
                                  where tp.TaskDefinitionId == task.TaskDefinitionId
                                  select tp).ToList();

                Assert.IsNotNull(task);
                Assert.AreEqual("This is the title", task.Title);
                Assert.AreEqual("This is the desc", task.Description);
                Assert.AreEqual(2, users.Count());
                Assert.IsTrue(HasUser(users, "CGRANT"));
                Assert.IsTrue(HasUser(users, "PNEWMAN"));
                Assert.AreEqual(5, parameters.Count());
                Assert.IsTrue(HasParameter(parameters, "Test>>title1"));
                Assert.IsTrue(HasParameter(parameters, "Test>>desc1"));
                Assert.IsTrue(HasParameter(parameters, "Test>>task1"));
                Assert.IsTrue(HasParameter(parameters, "Test>>task2"));
            }
        }

        [TestMethod]
        public void When_A_Task_Is_Completed_The_Properties_Are_Deleted()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>WfPropName", Value = "WfPropValue"}
                });

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}"
                },
                new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>TaskPropName", Value = "TaskPropValue"}
                }
            );

            CompleteTask(taskOid, "OK");

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var task = (from td in uowTasks.TaskDefinitions.AsQueryable()
                            where td.TaskOid == taskOid
                            select td).FirstOrDefault();

                var props = (from p in uowTasks.Properties.AsQueryable()
                             where p.Name == "Test>>WfPropName" || p.Name == "Test>>TaskPropName"
                             select p);

                Assert.IsNull(task);
                Assert.AreEqual(props.Count(), 1);
                Assert.AreEqual(props.First().Name, "Test>>WfPropName");
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Assign_A_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}"
                },
                null
            );

            AssignTask("cgrant", taskOid);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var tkd = (from t in uowTasks.TaskDefinitions.AsQueryable()
                           where t.TaskOid == taskOid
                           select t).FirstOrDefault();
                var tku = (from u in uowTasks.TaskUsers.AsQueryable()
                           where u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == "CGRANT"
                           select u).FirstOrDefault();

                Assert.AreEqual("cgrant", tkd.AcceptedBy);
                Assert.IsNotNull(tku);
                Assert.IsTrue(tku.InUse);
            }
        }

        [TestMethod]
        public void When_A_Task_Is_Assigned_It_Should_Disappear_From_Other_Users_List()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}"
                },
                null
            );

            AssignTask("cgrant", taskOid);

            var users = GetNextTasksForUser("PNEWMAN", workflowOid);

            Assert.IsTrue(users.Count() == 0);
        }

        [TestMethod]
        public void Users_Should_Not_See_Tasks_For_Workflows_In_Domain_They_Dont_Belong()
        {
            var workflowOid1 = CreateWorkflow("Acme1", null);
            var workflowOid2 = CreateWorkflow("Acme2", null);

            var taskOid1 = CreateTask(workflowOid1,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}"
                },
                null
            );

            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT" } };
            var fin = new GetUsersByRolesResponse { Users = new[] { "HBOGART", "KNOVAK", "MMONROE" } };

            var taskOid2 = CreateTask(workflowOid2,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}"
                },
                null, dev, fin
            );

            var users = GetNextTasksForUser("PNEWMAN", new[] { workflowOid1, workflowOid2 });

            Assert.AreEqual(users.Count(), 1);
        }

        [TestMethod]
        public void Should_Be_Able_To_Hand_Over_A_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );
            AssignTask("cgrant", taskOid);
            HandOverTask("hbogart", taskOid);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var tkd = (from t in uowTasks.TaskDefinitions.AsQueryable()
                           where t.TaskOid == taskOid
                           select t).FirstOrDefault();
                var tku = (from u in uowTasks.TaskUserHandOvers.AsQueryable()
                           where u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == "hbogart"
                           select u).FirstOrDefault();

                Assert.AreEqual(tkd.AcceptedBy, string.Empty);
                Assert.AreEqual(tkd.HandedOverStatus, Flow.Tasks.Data.DAL.Task.HandOverStatus.HandedOver.ToString());
                Assert.IsTrue(tku.InUse);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Hand_Over_A_Task_Which_Was_Handed_Over()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );
            AssignTask("cgrant", taskOid);
            HandOverTask("hbogart", taskOid);
            AssignTask("hbogart", taskOid);
            HandOverTask("hbogart", taskOid);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var tkd = (from t in uowTasks.TaskDefinitions.AsQueryable()
                           where t.TaskOid == taskOid
                           select t).FirstOrDefault();
                var tku = (from u in uowTasks.TaskUserHandOvers.AsQueryable()
                           where u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == "hbogart"
                           select u).FirstOrDefault();

                Assert.AreEqual(tkd.AcceptedBy, string.Empty);
                Assert.AreEqual(tkd.HandedOverStatus, Flow.Tasks.Data.DAL.Task.HandOverStatus.HandedOver.ToString());
                Assert.IsTrue(tku.InUse);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Give_Back_A_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );
            AssignTask("cgrant", taskOid);
            GiveBackTask(taskOid);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var tkd = (from t in uowTasks.TaskDefinitions.AsQueryable()
                           where t.TaskOid == taskOid
                           select t).FirstOrDefault();
                var tku = (from u in uowTasks.TaskUsers.AsQueryable()
                           where u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == "cgrant"
                           select u).FirstOrDefault();

                Assert.AreEqual(tkd.AcceptedBy, string.Empty);
                Assert.AreEqual(tkd.HandedOverStatus, Flow.Tasks.Data.DAL.Task.HandOverStatus.None.ToString());
                Assert.IsFalse(tku.InUse);
            }
        }

        [TestMethod]
        public void When_Task_Is_Created_Should_Get_The_Users_Who_Can_Complete_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );

            var users = GetUsersForTask(taskOid);

            Assert.IsTrue(users.Any(u => u.Equals("cgrant", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(users.Any(u => u.Equals("pnewman", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void When_Task_Is_Assigned_Should_Get_The_Users_To_Handover_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "cgrant;{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );

            AssignTask("cgrant", taskOid);
            var users = GetHandOverUsersForTask(taskOid);

            Assert.IsTrue(users.Any(u => u.Equals("hbogart", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(users.Any(u => u.Equals("knovak", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(users.Any(u => u.Equals("mmonroe", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_The_Tasks_Assigned_To_User()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );

            var taskOid2 = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );

            var taskOid3 = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "hbogart",
                    HandOverUsers = "{r.Finance}"
                },
                null
            );

            var tasks = GetNextTasksForUser("cgrant", workflowOid);
            Assert.AreEqual(tasks.Count(), 2);
        }

        [TestMethod]
        public void Should_Add_A_Trace_To_A_Workflow()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            AddTrace(workflowOid, "this is a trace", "cgrant", TraceEventType.Error);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var traceType = TraceEventType.Error.ToString();
                var traces = from te in uowTasks.WorkflowTraces.AsQueryable()
                             .Include("TraceEvent")
                             where te.WorkflowDefinition.WorkflowOid == workflowOid && te.TraceEvent.Type == traceType
                             select te;

                Assert.AreEqual(traces.Count(), 1);
                Assert.AreEqual(traces.First().User, "cgrant");
                Assert.AreEqual(traces.First().Message, "this is a trace");
                Assert.AreEqual(traces.First().TraceEvent.Type, traceType);
            }
        }

        [TestMethod]
        public void Should_Read_All_The_Traces_Of_A_Workflow()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            AddTrace(workflowOid, "this is a activity trace", "cgrant", TraceEventType.Activity);
            AddTrace(workflowOid, "this is a debug trace", "cgrant", TraceEventType.Debug);

            var traces = GetTraceForWorkflow(workflowOid, TraceEventType.Debug);

            Assert.AreEqual(traces.Count(), 1);
            Assert.AreEqual(traces.First().User, "cgrant");
            Assert.AreEqual(traces.First().Message, "this is a debug trace");
            Assert.AreEqual(traces.First().Type, TraceEventType.Debug.ToString());
        }

        [TestMethod]
        public void Should_Be_Able_To_Complete_A_Workflow()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            CompleteWorkflow(workflowOid, WorkflowStatusType.Completed);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfd = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                           .Include("WorkflowStatus")
                           where w.WorkflowOid == workflowOid
                           select w).FirstOrDefault();

                Assert.AreEqual(wfd.WorkflowStatus.Status, WorkflowStatusType.Completed.ToString());
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Complete_A_Workflow_With_Children()
        {
            var workflowOid1 = CreateWorkflow("Acme1", null);
            var workflowOid2 = CreateWorkflow(workflowOid1, "Acme1", null);

            CompleteWorkflow(workflowOid1, WorkflowStatusType.Completed);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfd = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                           .Include("WorkflowStatus")
                           where w.WorkflowOid == workflowOid1
                           select w).FirstOrDefault();

                Assert.AreEqual(wfd.WorkflowStatus.Status, WorkflowStatusType.Completed.ToString());
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_The_Traces_For_A_Workflow_With_Children()
        {
            var workflowOid1 = CreateWorkflow("Acme1", null);
            var workflowOid2 = CreateWorkflow(workflowOid1, "Acme1", null);
            var workflowOid3 = CreateWorkflow(workflowOid2, "Acme1", null);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfd = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                           where w.WorkflowOid == workflowOid1
                           select w).FirstOrDefault();
            }

            var traces = GetTraceForWorkflow(workflowOid1, TraceEventType.Activity);

            Assert.AreEqual(traces.Count(), 3);
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_The_Configuration_For_A_WorkflowCode()
        {
            var res = GetWorkflowConfiguration(WORKFLOW_CODE, DateTime.Now);

            Assert.IsNotNull(res);
            Assert.AreEqual(res.ServiceEndPoint, "BasicHttpBinding_IFlowTasksOperations");
        }


        [TestMethod]
        public void Should_Be_Able_To_Get_The_Result_Of_A_Workflow()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            CompleteWorkflow(workflowOid, WorkflowStatusType.Completed);

            var res = GetResult(workflowOid);

            Assert.IsNotNull(res);
            Assert.AreEqual("OK", res.Result);
            Assert.AreEqual(WorkflowStatusType.Completed.ToString(), res.Status);

        }

        [TestMethod]
        public void Should_Be_Able_To_Check_A_Workflow_Progress()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            CompleteWorkflow(workflowOid, WorkflowStatusType.Completed);

            var res = IsInProgress(workflowOid);

            Assert.IsFalse(res);
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_The_WorkflowInfos()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var workflowInfos = GetWorkflow(workflowOid, "Acme1", "cgrant", "admin");

            Assert.AreEqual(1, workflowInfos.Count());
            Assert.AreEqual(workflowOid.ToString(), workflowInfos.First().WorkflowId);
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_The_WorkflowInfos_Only_For_User_Domains()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var workflowInfos = GetWorkflow(workflowOid, "zzz", "cgrant", "admin");

            Assert.AreEqual(0, workflowInfos.Count());
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_A_WorkflowInfo_Passing_Only_Worflow_Id()
        {
            var workflowOid = CreateWorkflow("Acme1", null);

            var workflowInfos = GetWorkflow(workflowOid, "Acme1", "", "");

            Assert.AreEqual(1, workflowInfos.Count());
            Assert.AreEqual(workflowOid, workflowInfos.First().WorkflowOid);
        }

        [TestMethod]
        public void Should_Be_Able_To_Delete_A_Workflow()
        {
            var workflowOid1 = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>prop1", Value = "val1"},
                    new PropertyInfo{Name = "Test>>prop2", Value = "val2"}
                });
            var workflowOid2 = CreateWorkflow(workflowOid1, "Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>prop3", Value = "val3"},
                    new PropertyInfo{Name = "Test>>prop4", Value = "val4"}
                });

            DeleteWorkflow(workflowOid1);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var wfd1 = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                            where w.WorkflowOid == workflowOid1
                            select w).FirstOrDefault();

                var wfd2 = (from w in uowTasks.WorkflowDefinitions.AsQueryable()
                            where w.WorkflowOid == workflowOid2
                            select w).FirstOrDefault();

                var param1 = (from p in uowTasks.WorkflowInParameters.AsQueryable()
                              where p.WorkflowDefinition.WorkflowOid == workflowOid1
                              select p).ToList();

                var param2 = (from p in uowTasks.WorkflowInParameters.AsQueryable()
                              where p.WorkflowDefinition.WorkflowOid == workflowOid2
                              select p).ToList();

                Assert.IsNull(wfd1);
                Assert.IsNull(wfd2);
                Assert.AreEqual(0, param1.Count());
                Assert.AreEqual(0, param2.Count());
            }


        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Parameters_For_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>prop1", Value = "val1"},
                    new PropertyInfo{Name = "Test>>prop2", Value = "val2"}
                });

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}"
                },
                new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>TaskProp1", Value = "val3"}
                }
            );

            var res = GetParameters(taskOid);

            Assert.AreEqual(4, res.Count());
        }

        [TestMethod]
        public void Should_Be_Able_To_Create_A_Notification()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>title1", Value = "the title"},
                    new PropertyInfo{Name = "Test>>desc1", Value = "the desc"}
                });

            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            var fin = new GetUsersByRolesResponse { Users = new[] { "HBOGART", "KNOVAK", "MMONROE" } };

            var notificationOid = CreateNotification(workflowOid,
                new NotificationInfo
                {
                    AssignedToUsers = "{r.Dev}",
                    Title = "This is {p.Test>>title1}",
                    Description = "This is {p.Test>>desc1}",
                },
                dev, fin
            );

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var notification = (from td in uowTasks.TaskDefinitions.AsQueryable()
                                    where td.TaskOid == notificationOid
                                    select td);

                Assert.IsNotNull(notification);
                Assert.AreEqual(2, notification.Count());

                var users1 = (from tu in uowTasks.TaskUsers.AsQueryable()
                              where tu.TaskDefinitionId == notification.FirstOrDefault().TaskDefinitionId
                              select tu).ToList();
                var users2 = (from tu in uowTasks.TaskUsers.AsQueryable()
                              where tu.TaskDefinitionId == notification.OrderBy(n => n.TaskDefinitionId)
                                .Skip(1).FirstOrDefault().TaskDefinitionId
                              select tu).ToList();

                Assert.AreEqual(1, users1.Count());
                Assert.IsTrue(HasUser(users1, "CGRANT"));
                Assert.AreEqual("This is {p.Test>>title1}", notification.FirstOrDefault().Title);
                Assert.AreEqual("CGRANT", notification.FirstOrDefault().AcceptedBy);
                Assert.AreEqual(1, users2.Count());
                Assert.IsTrue(HasUser(users2, "PNEWMAN"));
                Assert.AreEqual("This is {p.Test>>title1}", notification.OrderBy(n => n.TaskDefinitionId)
                    .Skip(1).FirstOrDefault().Title);
                Assert.AreEqual("PNEWMAN", notification.OrderBy(n => n.TaskDefinitionId)
                    .Skip(1).FirstOrDefault().AcceptedBy);
            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Report_For_User_Task()
        {
            var res = ReportUserTask();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Should_Be_Able_To_Insert_A_Sketch()
        {
            var newOid = Guid.NewGuid();

            CreateSketch("MySketch", newOid, "pnewman", SketchStatusType.Saved);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var sc = (from s in uowTasks.SketchConfigurations.AsQueryable()
                          .Include("SketchStatus")
                          where s.Name == "MySketch"
                              select s).ToList();

                Assert.IsNotNull(sc);
                Assert.AreEqual(1, sc.Count());
                Assert.AreEqual("pnewman", sc[0].ChangedBy);
                Assert.AreEqual(newOid, sc[0].XamlxOid);
                Assert.AreEqual(SketchStatusType.Saved.ToString(), sc[0].SketchStatus.Status);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Update_A_Sketch()
        {
            var newOid = Guid.NewGuid();

            CreateSketch("MySketch", newOid, "pnewman", SketchStatusType.Saved);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var sc = (from s in uowTasks.SketchConfigurations.AsQueryable()
                          .Include("SketchStatus")
                          where s.Name == "MySketch"
                          select s).ToList();

                Assert.IsNotNull(sc);
                Assert.AreEqual(1, sc.Count());
                Assert.AreEqual("pnewman", sc[0].ChangedBy);
                Assert.AreEqual(newOid, sc[0].XamlxOid);
                Assert.AreEqual(SketchStatusType.Saved.ToString(), sc[0].SketchStatus.Status);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Topics()
        {
            var topics = GetTopics("hbogart", DateTime.MinValue, DateTime.MaxValue);

            //Assert.IsNotNull(topics);
            ////Assert.AreEqual(1, topics.Count());
            //Assert.AreEqual("Hoax DJ to fight Austereo at Fair Work", topics.First().Title);
            //Assert.AreEqual("One of the radio hosts who made the royal prank call to a London nurse is taking her employer to the workplace relations tribunal.", topics.First().Message.Message);
            //Assert.AreEqual("Read", topics.First().Message.Status.ToString());
            //Assert.AreEqual("hbogart", topics.First().Message.From);
            //Assert.AreEqual("{r.dev}", topics.First().Message.To);
            //Assert.AreEqual("12/07/2013", topics.First().Message.When.ToShortDateString());
            //Assert.AreEqual("filefake.txt", topics.First().Message.Documents.First().FileName);
        }

        [TestMethod]
        public void Should_Be_Able_To_Create_A_Topic()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            //var fin = new GetUsersByRolesResponse { Users = new[] { "HBOGART", "KNOVAK", "MMONROE" } };

            CreateTopic("Test>>This is a title", "this is a message", "CGRANT", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = (from t in uowTasks.Topics.AsQueryable()
                            where t.Title == "Test>>This is a title"
                            select t).FirstOrDefault();
                Assert.IsNotNull(topic);

                var message = (from m in uowTasks.TopicMessages.AsQueryable()
                            where m.TopicId == topic.TopicId
                            select m).FirstOrDefault();
                Assert.IsNotNull(message);

                var users = (from u in uowTasks.TopicUsers.AsQueryable()
                              where u.TopicMessageId == message.TopicMessageId
                              select u).ToList();

                Assert.AreEqual("this is a message", message.Message);
                Assert.AreEqual("CGRANT", message.From);
                Assert.AreEqual("rredford;CGRANT;{r.Dev}", message.To);
                Assert.IsTrue(message.IsTopic);
                Assert.AreEqual(3, users.Count());
                Assert.AreEqual("rredford", users.First().User);
                Assert.AreEqual("CGRANT", users.Skip(1).First().User);
                Assert.AreEqual("PNEWMAN", users.Skip(2).First().User);

            }
            
        }

        [TestMethod]
        public void Should_Be_Able_To_Create_A_Topic_Reply()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;cgrant;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);
                
                CreateReply(topic.TopicId, "this is a reply", "rredford", "", new List<TopicAttachmentInfo>(), dev);

                var message = uowTasks.TopicMessages.FirstOrDefault(m => m.TopicId == topic.TopicId && !m.IsTopic);
                Assert.IsNotNull(message);

                var users = uowTasks.TopicUsers.Find(u => u.TopicMessageId == message.TopicMessageId).ToList();

                Assert.AreEqual("this is a reply", message.Message);
                Assert.AreEqual("rredford", message.From);
                Assert.AreEqual("rredford;cgrant;{r.Dev}", message.To);
                Assert.IsFalse(message.IsTopic);
                Assert.AreEqual(3, users.Count());
                Assert.AreEqual("rredford", users.First().User);
                Assert.AreEqual("cgrant", users.Skip(1).First().User);
                Assert.AreEqual("PNEWMAN", users.Skip(2).First().User);
            }
        }

        [TestMethod]
        public void When_User_Create_A_Reply_It_Should_Appear_To_The_Topic_Owner()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "hbogart", "{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                CreateReply(topic.TopicId, "this is a reply", "CGRANT", "", new List<TopicAttachmentInfo>(), dev);

                var message = uowTasks.TopicMessages.FirstOrDefault(m => m.TopicId == topic.TopicId && !m.IsTopic);
                Assert.IsNotNull(message);

                var users = uowTasks.TopicUsers.Find(u => u.TopicMessageId == message.TopicMessageId).ToList();

                Assert.AreEqual("CGRANT", message.From);
                Assert.AreEqual("{r.Dev};hbogart", message.To);
                Assert.AreEqual(3, users.Count());
                Assert.AreEqual("CGRANT", users.First().User);
                Assert.AreEqual("PNEWMAN", users.Skip(1).First().User);
                Assert.AreEqual("hbogart", users.Skip(2).First().User);
            }
        }

        [TestMethod]
        public void Should_Be_Able_To_Get_All_Topics_For_User_With_Replies()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                CreateReply(topic.TopicId, "this is a reply", "rredford", "", new List<TopicAttachmentInfo>(), dev);

                var topics = GetTopics("CGRANT", DateTime.Now.AddMonths(-1), DateTime.Now);

                var statusNew = TopicStatusType.New.ToString();
                var users = uowTasks.TopicUsers.Find(u => u.TopicStatus.Status == statusNew && u.User == "CGRANT" && u.TopicMessage.Topic.Title.Contains("Test>>"));

                Assert.IsNotNull(topics);
                
                topics = topics.Where(t => !string.IsNullOrWhiteSpace(t.Title) && t.Title.Contains("Test>>"));

                Assert.AreEqual(1, topics.Count());
                Assert.AreEqual(TopicStatusType.New, topics.First().Status); 
                Assert.AreEqual("Test>>This is a title", topics.First().Title);
                Assert.AreEqual("this is a message", topics.First().Message.Message);
                Assert.AreEqual("Read", topics.First().Message.Status.ToString());
                Assert.AreEqual("cgrant", topics.First().Message.From);
                Assert.AreEqual("rredford;CGRANT;{r.Dev}", topics.First().Message.To);
                Assert.AreEqual(1, topics.First().Replies.Count());
                Assert.AreEqual(0, users.Count());

            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Get_All_Topics_For_User_Without_Replies()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                CreateReply(topic.TopicId, "this is a reply", "rredford", "", new List<TopicAttachmentInfo>(), dev);

                var topics = GetTopics("CGRANT", DateTime.Now.AddMonths(-1), DateTime.Now, false);

                var statusNew = TopicStatusType.New.ToString();
                var users = uowTasks.TopicUsers.Find(u => u.TopicStatus.Status == statusNew && u.User == "CGRANT" && u.TopicMessage.Topic.Title.Contains("Test>>"));

                Assert.IsNotNull(topics);

                topics = topics.Where(t => !string.IsNullOrWhiteSpace(t.Title) && t.Title.Contains("Test>>"));

                Assert.AreEqual(1, topics.Count());
                Assert.AreEqual(TopicStatusType.New, topics.First().Status); 
                Assert.AreEqual("Test>>This is a title", topics.First().Title);
                Assert.AreEqual("this is a message", topics.First().Message.Message);
                Assert.AreEqual(TopicStatusType.Read, topics.First().Message.Status);
                Assert.AreEqual("cgrant", topics.First().Message.From);
                Assert.AreEqual("rredford;CGRANT;{r.Dev}", topics.First().Message.To);
                Assert.AreEqual(0, topics.First().Replies.Count());
                Assert.AreEqual(0, users.Count());

            }

        }

        [TestMethod]
        public void When_User_Add_New_Reply_Topics_Count_Should_Set_To_One()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };

            // Make sure all topic set to read
            GetTopics("rredford", DateTime.Now.AddMonths(-1), DateTime.Now, false);
            
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            var topicCount = GetTopicCount("rredford");

            Assert.AreEqual(1, topicCount);

            // Make sure all topic set to read
            GetTopics("rredford", DateTime.Now.AddMonths(-1), DateTime.Now, false);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                CreateReply(topic.TopicId, "this is a reply", "cgrant", "", new List<TopicAttachmentInfo>(), dev);

                topicCount = GetTopicCount("rredford");
                Assert.AreEqual(1, topicCount);
            }

            GetTopics("rredford", DateTime.Now.AddMonths(-1), DateTime.Now, false);
            topicCount = GetTopicCount("rredford");
            Assert.AreEqual(0, topicCount);
        }


        [TestMethod]
        public void Should_Be_Able_To_Get_All_Replies()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                for (var i = 0; i < 30; i++)
                {
                    CreateReply(topic.TopicId, "this is a reply-" + i, "rredford", "", new List<TopicAttachmentInfo>(), dev);
                }
                var replies = GetReplies(topic.TopicId, "CGRANT", DateTime.Now.AddMonths(-1), DateTime.Now, RepliesShowType.All);

                var statusNew = TopicStatusType.New.ToString();
                var users = uowTasks.TopicUsers.Find(u => u.TopicStatus.Status == statusNew && u.User == "CGRANT" && u.TopicMessage.Topic.Title.Contains("Test>>"));

                Assert.IsNotNull(replies);

                Assert.AreEqual(30, replies.Count());
                Assert.AreEqual("this is a reply-0", replies.First().Message);
                Assert.AreEqual(0, users.Count());
            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Recent_Replies()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                for (var i = 0; i < 30; i++)
                {
                    CreateReply(topic.TopicId, "this is a reply-" + i, "rredford", "", new List<TopicAttachmentInfo>(), dev);
                }
                var replies = GetReplies(topic.TopicId, "CGRANT", DateTime.Now.AddMonths(-1), DateTime.Now, RepliesShowType.Recent);

                var statusNew = TopicStatusType.New.ToString();
                var users = uowTasks.TopicUsers.Find(u => u.TopicStatus.Status == statusNew && u.User == "CGRANT" && u.TopicMessage.Topic.Title.Contains("Test>>"));

                Assert.IsNotNull(replies);

                Assert.AreEqual(20, replies.Count());
                Assert.AreEqual("this is a reply-0", replies.First().Message);
                Assert.AreEqual("this is a reply-19", replies.Last().Message);
                Assert.AreEqual(0, users.Count());
            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Get_Old_Replies()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                for (var i = 0; i < 30; i++)
                {
                    CreateReply(topic.TopicId, "this is a reply-" + i, "rredford", "", new List<TopicAttachmentInfo>(), dev);
                }
                var replies = GetReplies(topic.TopicId, "CGRANT", DateTime.Now.AddMonths(-1), DateTime.Now, RepliesShowType.Old);

                var statusNew = TopicStatusType.New.ToString();
                var users = uowTasks.TopicUsers.Find(u => u.TopicStatus.Status == statusNew && u.User == "CGRANT" && u.TopicMessage.Topic.Title.Contains("Test>>"));

                Assert.IsNotNull(replies);

                Assert.AreEqual(10, replies.Count());
                Assert.AreEqual("this is a reply-20", replies.First().Message);
                Assert.AreEqual("this is a reply-29", replies.Last().Message);
                Assert.AreEqual(0, users.Count());
            }

        }

        [TestMethod]
        [Ignore]
        public void Should_Be_Able_To_Search_For_Topic()
        {
            var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            CreateTopic("Test>>This is a title", "this is a message for topic to search. pattern is: **tag**", "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);

            using (var uowTasks = new FlowTasksUnitOfWork())
            {
                var topic = uowTasks.Topics.FirstOrDefault(t => t.Title == "Test>>This is a title");
                Assert.IsNotNull(topic);

                CreateReply(topic.TopicId, "this is a reply", "rredford", "", new List<TopicAttachmentInfo>(), dev);

                var topics = SearchTopic("CGRANT", "**tag**");

                Assert.IsNotNull(topics);

                Assert.AreEqual(1, topics.Count());

            }

        }

        [TestMethod]
        public void Should_Be_Able_To_Search_For_A_Task()
        {
            var workflowOid = CreateWorkflow("Acme1", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>prop1", Value = "val1"},
                    new PropertyInfo{Name = "Test>>prop2", Value = "val2"}
                });

            var taskOid = CreateTask(workflowOid,
                new TaskInfo
                {
                    AssignedToUsers = "{r.Dev}"
                },
                new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>TaskProp1", Value = "val3"}
                }
            );

            var tasks = SearchForTask("tcode", "", new List<PropertyInfo>
                {
                    new PropertyInfo{Name = "Test>>TaskProp1", Value = "val3"}
                });

            Assert.IsNotNull(tasks);
            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual("tcode", tasks.First().TaskCode);
        }

        [TestMethod]
        public void When_Cannot_Found_Tasks_Should_Return_Empty_Result()
        {
            List<TaskInfo> tasks = new List<TaskInfo>();
            var arrayOfTasks = tasks.ToArray();

            Assert.IsNotNull(arrayOfTasks);
            Assert.AreEqual(0, arrayOfTasks.Count());
        }

        [TestMethod]
        public void Should_Be_Able_To_Search_For_Topics_Using_Full_Text_Index()
        {
            //var dev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };

            //Random rnd = new Random();
            //string line;
            //using (StreamReader sr = new StreamReader("..\\..\\fortopics.txt"))
            //{
            //    line = sr.ReadToEnd();
            //}

            //for (int i = 0; i < 10000; i++)
            //{
            //    var len = line.Length;
            //    var topicLen = rnd.Next(20, 150);
            //    var startFrom = rnd.Next(len - topicLen);
            //    var topic = line.Substring(startFrom, topicLen);

            //    CreateTopic("", topic, "cgrant", "rredford;CGRANT;{r.Dev}", new List<TopicAttachmentInfo>(), dev);
            //}

            var res = SearchTopic("cgrant", "submit test").OrderByDescending(t => t.Rank).ToList();

        }


        private IEnumerable<WorkflowInfo> GetWorkflow(Guid workflowOid, string userDomain, string user, string role)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var domains = new GetDomainsForUserResponse { Domains = new[] { userDomain } };

            mockUsersService.Setup(x => x.GetDomainsForUser(Moq.It.IsAny<GetDomainsForUserRequest>()))
                .Returns(domains);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, mockUsersService.Object);
            int tot;
            IEnumerable<string> workflowCodes;
            return w.GetWorkflows(workflowOid, "", "", false, null, null, "cgrant", "admin", 0, 10, out tot, out workflowCodes);
        }

        private bool IsInProgress(Guid workflowOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            return w.IsWorkflowInProgress(workflowOid);
        }

        private WorkflowResultInfo GetResult(Guid workflowOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            return w.GetWorkflowResult(workflowOid);
        }

        private Guid CreateWorkflow(string domain, IEnumerable<PropertyInfo> properties)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, mockUsersService.Object);

            var oid = Guid.NewGuid();
            w.CreateWorkflow(oid, Guid.Empty, WORKFLOW_CODE, domain, properties);

            return oid;
        }

        private void AddWorkflow(string code, string serviceUrl, string bindingConfiguration, string serviceEndpoint)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, mockUsersService.Object);

            w.AddWorkflow(code, serviceUrl, bindingConfiguration, serviceEndpoint);
        }

        private void CreateSketch(string name, Guid oid, string changedBy, SketchStatusType status)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, mockUsersService.Object);

            w.SketchWorkflow(name, oid, changedBy, status);

        }

        private Guid CreateWorkflow(Guid parent, string domain, IEnumerable<PropertyInfo> properties)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            var oid = Guid.NewGuid();
            w.CreateWorkflow(oid, parent, WORKFLOW_CODE, domain, properties);

            return oid;
        }

        private Guid CompleteWorkflow(Guid oid, WorkflowStatusType status)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            w.CompleteWorkflow(oid, status, "OK", "");

            return oid;
        }

        private Guid CreateTask(Guid workflowOid, TaskInfo taskInfo, IEnumerable<PropertyInfo> properties)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var isValidUserResponse = new IsValidUserResponse { IsValid = true };
            var getUsersByRolesResponseDev = new GetUsersByRolesResponse { Users = new[] { "CGRANT", "PNEWMAN" } };
            var getUsersByRolesResponseFin = new GetUsersByRolesResponse { Users = new[] { "HBOGART", "KNOVAK", "MMONROE" } };

            mockUsersService.Setup(x => x.IsValidUser(Moq.It.IsAny<IsValidUserRequest>()))
                .Returns(isValidUserResponse);
            mockUsersService.Setup(x => x.GetUsersByRoles(Moq.It.IsAny<GetUsersByRolesRequest>()))
                .Returns((GetUsersByRolesRequest param) => param.Roles.First() == "Dev" ?
                    getUsersByRolesResponseDev : getUsersByRolesResponseFin);

            var taskOid = Guid.NewGuid();
            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);
            taskInfo.TaskOid = taskOid;
            taskInfo.TaskCode = "tcode";
            taskInfo.UiCode = "uicode";

            t.CreateTask(workflowOid, taskInfo, properties);

            return taskOid;
        }

        private Guid CreateTask(Guid workflowOid, TaskInfo taskInfo, IEnumerable<PropertyInfo> properties, GetUsersByRolesResponse dev, GetUsersByRolesResponse fin)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var isValidUserResponse = new IsValidUserResponse { IsValid = true };
            var getUsersByRolesResponseDev = dev;
            var getUsersByRolesResponseFin = fin;

            mockUsersService.Setup(x => x.IsValidUser(Moq.It.IsAny<IsValidUserRequest>()))
                .Returns(isValidUserResponse);
            mockUsersService.Setup(x => x.GetUsersByRoles(Moq.It.IsAny<GetUsersByRolesRequest>()))
                .Returns((GetUsersByRolesRequest param) => param.Roles.First() == "Dev" ?
                    getUsersByRolesResponseDev : getUsersByRolesResponseFin);

            var taskOid = Guid.NewGuid();
            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);
            taskInfo.TaskOid = taskOid;
            taskInfo.TaskCode = "tcode";
            taskInfo.UiCode = "uicode";

            t.CreateTask(workflowOid, taskInfo, properties);

            return taskOid;
        }

        private Guid CreateNotification(Guid workflowOid, NotificationInfo notificationInfo, GetUsersByRolesResponse dev, GetUsersByRolesResponse fin)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var isValidUserResponse = new IsValidUserResponse { IsValid = true };
            var getUsersByRolesResponseDev = dev;
            var getUsersByRolesResponseFin = fin;

            mockUsersService.Setup(x => x.IsValidUser(Moq.It.IsAny<IsValidUserRequest>()))
                .Returns(isValidUserResponse);
            mockUsersService.Setup(x => x.GetUsersByRoles(Moq.It.IsAny<GetUsersByRolesRequest>()))
                .Returns((GetUsersByRolesRequest param) => param.Roles.First() == "Dev" ?
                    getUsersByRolesResponseDev : getUsersByRolesResponseFin);

            var taskOid = Guid.NewGuid();
            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);
            notificationInfo.TaskOid = taskOid;

            t.CreateNotification(workflowOid, notificationInfo);

            return taskOid;
        }

        private void CompleteTask(Guid taskOid, string result)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            t.CompleteTask(taskOid, result, "");
        }


        private void AssignTask(string user, Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            t.AssignTaskTo(user, taskOid);
        }

        private void GiveBackTask(Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            t.GiveBackTask(taskOid);
        }

        private void HandOverTask(string user, Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            t.HandOverTaskTo(user, taskOid);
        }

        private IEnumerable<string> GetUsersForTask(Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            return t.GetUsersForTask(taskOid);
        }

        private IEnumerable<string> GetHandOverUsersForTask(Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            return t.GetHandOverUsersForTask(taskOid);
        }

        private IEnumerable<TaskInfo> GetNextTasksForUser(string user, Guid workflowOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            return t.GetNextTasksForUser(user, workflowOid, "", 0, 9999, string.Empty);
        }

        private IEnumerable<TaskInfo> GetNextTasksForUser(string user, IEnumerable<Guid> workflowOids)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var tracer = new Tracer();
            Task t = new Task(mockUsersService.Object, tracer);

            return t.GetNextTasksForUser(user, workflowOids, "", 0, 9999, string.Empty);
        }

        private void AddTrace(Guid workflowOid, string msg, string user, TraceEventType type)
        {
            Tracer t = new Tracer();
            t.Trace(workflowOid, ActionTrace.TaskCreated, "code", "result", user, msg, type);
        }

        private IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid workflowOid, TraceEventType type)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            return t.GetTraceForWorkflow(new[] { workflowOid }, type);
        }

        private WorkflowConfigurationInfo GetWorkflowConfiguration(string workflowCode, DateTime effectiveDate)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            return w.GetWorkflowConfiguration(workflowCode, effectiveDate);
        }

        private void DeleteWorkflow(Guid workflowOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            w.DeleteWorkflow(workflowOid);
        }

        private IEnumerable<PropertyInfo> GetParameters(Guid taskOid)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer t = new Tracer();
            Task task = new Task(t);

            return task.GetTaskParameters(taskOid);
        }

        private bool HasUser(List<TaskUser> tu, string user)
        {
            foreach (var u in tu)
            {
                if (u.User == user) return true;
            }

            return false;
        }

        private bool HasParameter(List<TaskInParameter> tp, string parameter)
        {
            foreach (var p in tp)
            {
                if (p.Property.Name == parameter) return true;
            }

            return false;
        }

        private IEnumerable<ReportUserTasksInfo> ReportUserTask()
        {
            Tracer t = new Tracer();
            Task task = new Task(t);
            Workflow w = new Workflow(task, t, null);

            return w.ReportUserTasks(null, null, null);
        }

        private IEnumerable<TopicInfo> GetTopics(string user, DateTime start, DateTime end, bool withReplies = true)
        {
            var userInfo = new GetUserResponse
            {
                User = new UserInfo
                {
                    PhotoPath = ""
                }
            };

            var mockUsersService = new Mock<IFlowUsersService>();

            mockUsersService.Setup(x => x.GetUser(Moq.It.IsAny<GetUserRequest>()))
                .Returns(userInfo);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            return msn.GetTopicsForUser(user, 0, start, end, "", "", 0 , 100, withReplies);
        }

        private int GetTopicCount(string user)
        {
            var userInfo = new GetUserResponse
            {
                User = new UserInfo
                {
                    PhotoPath = ""
                }
            };

            var mockUsersService = new Mock<IFlowUsersService>();

            mockUsersService.Setup(x => x.GetUser(Moq.It.IsAny<GetUserRequest>()))
                .Returns(userInfo);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            return msn.GetTopicCount(user);
        }

        private IEnumerable<TopicMessageInfo> GetReplies(int topicId, string user, DateTime start, DateTime end, RepliesShowType showType)
        {
            var userInfo = new GetUserResponse
            {
                User = new UserInfo
                {
                    PhotoPath = ""
                }
            };

            var mockUsersService = new Mock<IFlowUsersService>();

            mockUsersService.Setup(x => x.GetUser(Moq.It.IsAny<GetUserRequest>()))
                .Returns(userInfo);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            bool hasOldReplies;
            return msn.GetRepliesForUser(topicId, user, start, end, showType, out hasOldReplies);
        }

        private IEnumerable<SearchInfo> SearchTopic(string user, string pattern)
        {
            var userInfo = new GetUserResponse
            {
                User = new UserInfo
                {
                    PhotoPath = ""
                }
            };

            var mockUsersService = new Mock<IFlowUsersService>();

            mockUsersService.Setup(x => x.GetUser(Moq.It.IsAny<GetUserRequest>()))
                .Returns(userInfo);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            return msn.SearchForTopics(user, pattern);
        }

        private void CreateTopic(string title, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments, GetUsersByRolesResponse users)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var isValidUserResponse = new IsValidUserResponse { IsValid = true };

            mockUsersService.Setup(x => x.IsValidUser(Moq.It.IsAny<IsValidUserRequest>()))
                .Returns(isValidUserResponse);
            mockUsersService.Setup(x => x.GetUsersByRoles(Moq.It.IsAny<GetUsersByRolesRequest>()))
                .Returns(users);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            msn.CreateTopic(title, message, from, to, attachments);
        }

        private void CreateReply(int topicId, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments, GetUsersByRolesResponse users)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            var isValidUserResponse = new IsValidUserResponse { IsValid = true };

            mockUsersService.Setup(x => x.IsValidUser(Moq.It.IsAny<IsValidUserRequest>()))
                .Returns(isValidUserResponse);
            mockUsersService.Setup(x => x.GetUsersByRoles(Moq.It.IsAny<GetUsersByRolesRequest>()))
                .Returns(users);

            Tracer t = new Tracer();
            Task task = new Task(t);
            Data.DAL.Topic msn = new Data.DAL.Topic(mockUsersService.Object);

            msn.CreateReply(topicId, message, from, to, attachments);
        }

        private IEnumerable<TaskInfo> SearchForTask(string taskCode, string user, IEnumerable<PropertyInfo> props)
        {
            var mockUsersService = new Mock<IFlowUsersService>();

            Tracer tracer = new Tracer();
            Task task = new Task(mockUsersService.Object, tracer);

            return task.SearchForTasks(taskCode, user, props);            
        }

        [TestMethod]
        public void Test_Regex()
        {
            string word = "{p.ti>>>tle1} This {p.title2} is {p.title3}";
            //var r = Regex.Replace(word,
            //    @"\{[a-z]+\.[a-z\d]+\}", "xxx", RegexOptions.IgnoreCase);

            var r = Regex.Matches(word,
                @"\{[a-z]+\.[^\s]+\}", RegexOptions.IgnoreCase);

            List<string> matches = new List<string>();

            foreach (System.Text.RegularExpressions.Match match in r)
            {
                foreach (Capture capture in match.Captures)
                {
                    Console.WriteLine("Index={0}, Value={1}", capture.Index, capture.Value);
                    matches.Add(capture.Value);
                }
            }

            Assert.AreEqual(matches.Count(), 3);
            Assert.AreEqual(matches[0], "{p.ti>>>tle1}");
            Assert.AreEqual(matches[1], "{p.title2}");
            Assert.AreEqual(matches[2], "{p.title3}");
        }

        [TestMethod]
        public void Test_SubRegex()
        {
            string word = "{p.title1}";

            var r = Regex.Match(word,
                @"^\{([a-z]+)\.([^\s]+)\}$", RegexOptions.IgnoreCase);

            var type = r.Groups[1].ToString();
            var value = r.Groups[2].ToString();

            Assert.AreEqual(type, "p");
            Assert.AreEqual(value, "title1");
        }
    }
}
