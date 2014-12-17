using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Flow.Tasks.Data.Core.Interfaces;
using Flow.Tasks.Data.DAL;
using Flow.Library;
using Flow.Tasks.Data;
using Flow.Tasks.Data.Infrastructure;
using Flow.Users.Data;
using Flow.Users.Data.Infrastructure;
using Flow.Users.Data.DAL;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Test
{
    [TestClass]
    public class WorkflowTest
    {

        [TestInitialize]
        public void CreateContext()
        {
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestMethod]
        public void Should_Be_Able_To_Restart_A_Workflow()
        {
            var uow = new Mock<IFlowTasksUnitOfWork>();
            var task = new Mock<ITask>();
            var tracer = new Mock<ITracer>();

            var traces = new List<WorkflowTraceInfo>();

            Guid newOid = Guid.Parse("11111111-2222-3333-4444-555555555555");
            Guid oldOid = Guid.Parse("39388C6E-63D5-4EFB-8102-B3038D8F7723");

            #region Traces
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCreated",
                Code = "SampleWf7",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("39388C6E-63D5-4EFB-8102-B3038D8F7723")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCreated",
                Code = "SampleWf1",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("91CF961B-A8D3-4930-8098-C8FAA02A7DAA")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "TaskCreated",
                Code = "ApproveThisTask",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("91CF961B-A8D3-4930-8098-C8FAA02A7DAA")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCreated",
                Code = "SampleWf1",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("38EA6C5C-8EE0-483D-94EA-494A79F1F913")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "TaskCreated",
                Code = "ApproveThisTask",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("38EA6C5C-8EE0-483D-94EA-494A79F1F913")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "TaskCompleted",
                Code = "ApproveThisTask",
                Message = "",
                Result = "OK",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("91CF961B-A8D3-4930-8098-C8FAA02A7DAA")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCompleted",
                Code = "SampleWf1",
                Message = "",
                Result = "OK",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("91CF961B-A8D3-4930-8098-C8FAA02A7DAA")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "TaskCompleted",
                Code = "ApproveThisTask",
                Message = "",
                Result = "OK",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("38EA6C5C-8EE0-483D-94EA-494A79F1F913")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCompleted",
                Code = "SampleWf1",
                Message = "",
                Result = "OK",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("38EA6C5C-8EE0-483D-94EA-494A79F1F913")
            });
            traces.Add(new WorkflowTraceInfo
            {
                Action = "WorkflowCompleted",
                Code = "SampleWf7",
                Message = "",
                Result = "",
                Type = "Activity",
                User = "",
                When = new DateTime(2012, 6, 28).ToString(),
                WorkflowOid = Guid.Parse("39388C6E-63D5-4EFB-8102-B3038D8F7723")
            });

            #endregion

            #region Tasks

            var taskInfo = new TaskInfo
                {
                    TaskCode = "",
                    TaskOid = Guid.Empty
                }; 
            
            #endregion

            tracer.Setup(tr => tr.GetTraceForWorkflow(oldOid))
                .Returns(traces);
            task.Setup(tk => tk.GetNextTasksForWorkflow(newOid))
                .Returns(new[] {taskInfo});
            task.Setup(tk => tk.CompleteTask(It.IsAny<Guid>(), "", ""));                

            var sut = new Workflow(task.Object, tracer.Object, null);

            //sut.RestartWorkflow(oldOid, newOid);

            
        }
        
    }
}
