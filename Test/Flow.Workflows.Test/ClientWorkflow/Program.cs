using System;
using System.Linq;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;

namespace ClientWorkflow
{
    class Program
    {
        static void Main()
        {

            //CreateWorkflows("SampleWf2", 5);

            ServiceCallsWf6(false);

            //RunTests();
            //StressTest();

        }

        private static void RunTests()
        {
            try
            {
                Console.WriteLine(@"Running the worflows.........");
                Console.WriteLine("");

                ServiceCallsWf1(true);
                ServiceCallsWf2(true);
                ServiceCallsWf3(true);
                ServiceCallsWf4(true);
                ServiceCallsWf4a(true);
                ServiceCallsWf5(true);
                ServiceCallsWf6(true);
                ServiceCallsWf7(true);
                ServiceCallsWf8(true);
                ServiceCallsWf9(true);

                Console.WriteLine(@"All the worflows are OK. Press a key to Exit");
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(@"ERROR found. Press a key to Exit");
                Console.WriteLine("");

                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private static void StressTest()
        {
            try
            {
                Console.WriteLine(@"Running the stress test .........");
                Console.WriteLine("");

                for (int loop = 0; loop < 100; loop++)
                {
                    Console.Write("loop: {0} -> ", loop + 1);
                    
                    var rndNum = new Random().Next(10);
                    switch (rndNum)
                    {
                        case 1: ServiceCallsWf1(true);
                            break;
                        case 2: ServiceCallsWf2(true);
                            break;
                        case 3: ServiceCallsWf3(true);
                            break;
                        case 4: ServiceCallsWf4(true);
                            break;
                        case 5: ServiceCallsWf5(true);
                            break;
                        case 6: ServiceCallsWf6(true);
                            break;
                        case 7: ServiceCallsWf7(true);
                            break;
                        case 8: ServiceCallsWf8(true);
                            break;
                        case 9: ServiceCallsWf9(true);
                            break;
                        default: ServiceCallsWf1(true);
                            break;
                    }
                }

                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine(@"ERROR found. Press a key to Exit");
                Console.WriteLine("");

                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
        
        private static void CreateWorkflows(string name, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Start(name);                
            }
        }

        private static void ServiceCallsWf1(bool async)
        {
            const string name = "SampleWf1";

            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "ApproveThisTask" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf2(bool async)
        {
            const string name = "SampleWf2";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "ApproveTask1" }, "OK");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "ApproveTask2" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf3(bool async)
        {
            const string name = "SampleWf3";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            GetTasks(workflowId);

            ProcessTask(workflowId, name, 2, new[] { "ParallelApprove1", "ParallelApprove2" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf4(bool async)
        {
            const string name = "SampleWf4";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);


            ProcessTask(workflowId, name, 1, new[] { "IfCondTask" }, "OK");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "IfBramchTask" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf4a(bool async)
        {
            const string name = "SampleWf4a";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);


            ProcessTask(workflowId, name, 1, new[] { "IfCondTaskA" }, "OK");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "IfBramchTask" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf5(bool async)
        {
            const string name = "SampleWf5";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "WhileTask" }, "FAIL");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "WhileTask" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf6(bool async)
        {
            const string name = "SampleWf6";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(5000);

            ProcessChildern(workflowId, name, 1, new[] { "ApproveThisTask" }, "OK");

            System.Threading.Thread.Sleep(5000);

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");

        }

        private static void ServiceCallsWf7(bool async)
        {
            const string name = "SampleWf7";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessChildern(workflowId, name, 1, new[] { "ApproveThisTask" }, "OK");

            System.Threading.Thread.Sleep(5000);
            //dal = new DalAccess();

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf8(bool async)
        {
            const string name = "SampleWf8";

            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "FlowChartVb1" }, "OK");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "FlowChartVb2" }, "OK");

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static void ServiceCallsWf9(bool async)
        {
            const string name = "SampleWf9";
            Console.Write(@"Running: " + name + @"... ");

            string workflowId = Start(name);

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "Step1Task" }, "OK");

            if (IsAsyncCall(async)) return;
            System.Threading.Thread.Sleep(2000);

            ProcessTask(workflowId, name, 1, new[] { "Step2Task" }, "OK");
            System.Threading.Thread.Sleep(2000);

            CheckResult(workflowId, name);

            Console.WriteLine(@"OK!");
        }

        private static string Start(string workflowCode)
        {
            string workflowId;
            var startWorkflowRequest = new StartWorkflowRequest
            {
                Domain = "google",
                WorkflowCode = workflowCode,
                WfRuntimeValues = new[] 
                    {
                        new WfProperty
                        {
                            Name = "Prop1",
                            Type = "S",
                            Value = "Val1"
                        }
                    }
            };

            StartWorkflowResponse startWorkflowResponse = null;
            using (var src = new FlowTasksService())
            {
                startWorkflowResponse = src.StartWorkflow(startWorkflowRequest);
            }

            System.Threading.Thread.Sleep(1000);

            workflowId = startWorkflowResponse.WorkflowId;

            return workflowId;
        }

        private static GetWorkflowChildrenResponse GetChildren(string workflowId)
        {
            var getWorkflowChildrenRequest = new GetWorkflowChildrenRequest
            {
                WorkflowOid = workflowId
            };

            GetWorkflowChildrenResponse children = null;
            using (var src = new FlowTasksService())
            {
                children = src.GetWorkflowChildren(getWorkflowChildrenRequest);
            }

            return children;
        }

        private static GetNextTasksForUserResponse GetTasks(string workflowId)
        {
            var getNextTasksForUserRequest = new GetNextTasksForUserRequest
            {
                User = "pnewman",
                WorkflowOid = Guid.Parse(workflowId)
            };

            GetNextTasksForUserResponse tasks = null;
            using (var src = new FlowTasksService("FlowTasksService_Endpoint"))
            {
                tasks = src.GetNextTasksForUser(getNextTasksForUserRequest);
            }

            return tasks;
        }

        private static void ApproveTask(TaskInfo task, string workflowId, string result)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var user = rnd.Next(2) == 0 ? "rredford" : "pnewman";

            var assignTaskRequest = new AssignTaskToRequest
            {
                TaskOid = task.TaskOid,
                User = user
            };

            var approveTaskRequest = new ApproveTaskRequest
            {
                TaskId = task.TaskOid.ToString(),
                CorrelationId = task.TaskCorrelationId,
                TaskCode = task.TaskCode,
                Result = result,
                UserName = user,
                WorkflowId = workflowId
            };

            using (var src = new FlowTasksService())
            {
                src.AssignTaskTo(assignTaskRequest);
                src.ApproveTask(approveTaskRequest);
            }

        }

        private static bool IsAsyncCall(bool async)
        {
            if (!async)
            {
                Console.WriteLine(@"press a key to approve task or x to Exit");
                var res = Console.ReadLine();
                if (res != null && res.ToLower() == "x") return true;
            }

            return false;
        }

        private static void ProcessChildern(string workflowId, string name, int taskCount, string[] taskName, string taskResult)
        {
            var children = GetChildren(workflowId);
            foreach (var c in children.Children)
            {
                ProcessTask(c, name, taskCount, taskName, taskResult);
            }
        }

        private static void ProcessTask(string workflowId, string name, int taskCount, string[] taskName, string taskResult)
        {
            var tasks = GetTasks(workflowId);

            if (tasks.Tasks.Count() != taskCount) throw new Exception(name + ": wrong task count found");

            foreach (var task in tasks.Tasks)
            {
                ApproveTask(task, workflowId, taskResult);
                if (!taskName.Contains(task.TaskCode)) throw new Exception(name + ": wrong task code");
            }
        }

        private static void CheckResult(string workflowId, string name)
        {
            GetTraceForWorkflowResponse traces = null;
            using (var src = new FlowTasksService())
            {
                traces = src.GetTraceForWorkflow(new GetTraceForWorkflowRequest { WorkflowOids = new[] { Guid.Parse(workflowId) } });
            }

            bool found = false;
            foreach (var trace in traces.Traces.Where(t => t.Type == TraceEventType.Activity.ToString()))
            {
                if (trace.Action == ActionTrace.WorkflowCompleted.ToString()) found = true;
            }
            if (!found) throw new Exception(name + ": workflow has not been completed");
        }
    }
}
