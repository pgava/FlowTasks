using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Tasks.Web.Controllers;
using Moq;
using Flow.Users.Contract;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Users.Contract.Message;
using Flow.Tasks.View.Models;
using System.Web.Mvc;

namespace TaskList
{
    [TestClass]
    public class TaskListTest
    {
        

        private TaskListController ArrangeTestForNavigation()
        {
            var userMock = new Mock<IFlowUsersService>();
            var taskMock = new Mock<IFlowTasksService>();
            var docMock = new Mock<IFlowDocsDocument>();

            var respTasks = new GetNextTasksForUserResponse();
            respTasks.Tasks = CreateTestTasks(10);

            var respProps = new GetPropertiesForTaskResponse();
            respProps.Properties = CreateTestProperties(1);

            var respCmt = new GetTraceForWorkflowResponse();
            respCmt.Traces = null;

            var respDomains = new GetDomainsForUserResponse();
            respDomains.Domains = new[] { "google" };

            taskMock.Setup(t => t.GetNextTasksForUser(Moq.It.IsAny<GetNextTasksForUserRequest>()))
                .Returns(respTasks);
            taskMock.Setup(t => t.GetTraceForWorkflow(Moq.It.IsAny<GetTraceForWorkflowRequest>()))
                .Returns(respCmt);
            taskMock.Setup(t => t.GetPropertiesForTask(Moq.It.IsAny<GetPropertiesForTaskRequest>()))
                .Returns(respProps);
            userMock.Setup(u => u.GetDomainsForUser(Moq.It.IsAny<GetDomainsForUserRequest>()))
                .Returns(respDomains);

            var sut = new TaskListController(userMock.Object, taskMock.Object, docMock.Object);
            sut.ControllerContext = MockUtil.GetMockedControllerContext();

            return sut;
        }

        private PropertyInfos CreateTestProperties(int len)
        {
            var props = new List<PropertyInfo>();

            for (int i = 0; i < len; i++)
            {
                props.Add(new PropertyInfo
                {
                    Name = "name" +i,
                    Value = "value" + i
                });
            }

            return new PropertyInfos(props);
        }

        private TaskInfo[] CreateTestTasks(int len)
        {
            var tasks = new List<TaskInfo>();

            for (int i = 0; i < len; i++)
            {
                tasks.Add(new TaskInfo
                {
                    TaskCode = "code" + i,
                    Title = "title" + i,
                    Description = "desc" + i
                });
            }
            return tasks.ToArray();
        }
    }
}
