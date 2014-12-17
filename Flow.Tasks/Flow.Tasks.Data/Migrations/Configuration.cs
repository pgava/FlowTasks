namespace Flow.Tasks.Data.Migrations
{
    using Contract.Message;
    using Core;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<FlowTasksEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        private const string REMOVE_PROC = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportTaskTime]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[ReportTaskTime]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportUserTaskCount]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[ReportUserTaskCount]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportUserTasks]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[ReportUserTasks]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReportWorkflowTime]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[ReportWorkflowTime]

IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[SearchTopicForUser]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[SearchTopicForUser]
";

        private const string REPORT_TASK_TIME_PROC = @"
create proc [dbo].[ReportTaskTime](
	@start as datetime,
	@end as datetime
	)
as
begin

select avg(datediff(SECOND, tcrt.[When], tcmp.[When])) as Duration, -- mi = minutes
	tcmp.Code as Task,
	wf.Code as Workflow
from 
	(
		select * from dbo.WorkflowTrace
		where Action in ('TaskCreated')
	) tcrt
	inner join 
		(
			select * from dbo.WorkflowTrace
			where Action in ('TaskCompleted') and
				Result not in ('Expired', 'Terminated')
		) tcmp
	on tcrt.Code = tcmp.Code and tcrt.WorkflowDefinitionId = tcmp.WorkflowDefinitionId
	inner join
	 (
		select wd.WorkflowDefinitionId, wc.Code from dbo.WorkflowDefinition wd
		inner join WorkflowCode wc
		on wd.WorkflowCodeId = wc.WorkflowCodeId		
	) wf 
	on tcmp.WorkflowDefinitionId = wf.WorkflowDefinitionId
where
	(@start is null or @start <= tcmp.[When]) and
	(@end is null or @end >= tcmp.[When])
group by tcmp.Code, wf.Code
order by tcmp.Code, wf.Code

end";
        private const string REPORT_USER_TASK_COUNT_PROC = @"
create proc [dbo].[ReportUserTaskCount](
	@start as datetime,
	@end as datetime
	)
as
begin

select lower([User]) as [User], Code as Task, COUNT(*) as [Count] from WorkflowTrace
where Action = 'TaskCompleted' and
	Result not in ('Expired', 'Terminated') and
	(@start is null or @start <= WorkflowTrace.[When]) and
	(@end is null or @end >= WorkflowTrace.[When])	
group by lower([User]), Code

end";

        private const string REPORT_USER_TASKS_PROC = @"
create proc [dbo].[ReportUserTasks](
	@start as datetime,
	@end as datetime
	)
as
begin

select lower([User]) as [User], COUNT(*) as TaskNo from WorkflowTrace
where Action = 'TaskCompleted' and
	Result not in ('Expired', 'Terminated') and
	(@start is null or @start <= WorkflowTrace.[When]) and
	(@end is null or @end >= WorkflowTrace.[When])
group by lower([User])

end";

        private const string REPORT_WORKFLOW_TIME_PROC = @"
create proc [dbo].[ReportWorkflowTime](
	@start as datetime,
	@end as datetime
	)
as
begin

select avg(datediff(SECOND, tcrt.[When], tcmp.[When])) as Duration, -- mi = minutes
	tcmp.Code as Workflow
from 
	(
		select * from dbo.WorkflowTrace
		where Action in ('WorkflowCreated')
	) tcrt
	inner join 
		(
			select * from dbo.WorkflowTrace
			where Action in ('WorkflowCompleted') and
				Result not in ('Expired', 'Terminated')
		) tcmp
	on tcrt.Code = tcmp.Code and tcrt.WorkflowDefinitionId = tcmp.WorkflowDefinitionId
where
	(@start is null or @start <= tcmp.[When]) and
	(@end is null or @end >= tcmp.[When])	
group by tcmp.Code
order by tcmp.Code

end";
        private const string TOPIC_SEARCH_FUNC = @"
create function SearchTopicForUser
(
	@username nvarchar(16),
    @keywords nvarchar(4000)
)
returns table
as
  return (
	select m.[TopicId], m.[Message], KEY_TBL.[Rank]
	from TopicMessage m
		inner join containstable(TopicMessage, ([Message]),@keywords) AS KEY_TBL
			on m.TopicMessageId = KEY_TBL.[KEY]
		inner join TopicUser u 
			on m.TopicMessageId = u.TopicMessageId
	where u.[User] = @username
		  )

";

        protected override void Seed(FlowTasksEntities context)
        {

            TraceEvent teDebug = CreateEvent(TraceEventType.Debug, "Debug");
            TraceEvent teActivity = CreateEvent(TraceEventType.Activity, "Activity");
            TraceEvent teInfo = CreateEvent(TraceEventType.Info, "Info");
            TraceEvent teError = CreateEvent(TraceEventType.Error, "Error");

            context.TraceEvents.AddOrUpdate(
                te => te.Type,
                teDebug,
                teActivity,
                teError,
                teInfo
                );

            WorkflowStatus wsInProgress = CreateStatus(WorkflowStatusType.InProgress, "InProgress");
            WorkflowStatus wsCompleted = CreateStatus(WorkflowStatusType.Completed, "Completed");
            WorkflowStatus wsAborted = CreateStatus(WorkflowStatusType.Aborted, "Aborted");
            WorkflowStatus wsTerminated = CreateStatus(WorkflowStatusType.Terminated, "Terminated");

            context.WorkflowStatuses.AddOrUpdate(
                ws => ws.Status,
                wsCompleted,
                wsAborted,
                wsInProgress,
                wsTerminated
                );

            SketchStatus skSaved = CreateSketchStatus("Saved", "Sketch Saved");
            SketchStatus skDeployedDev = CreateSketchStatus("DeployedDev", "Deployed to dev");
            SketchStatus skDeployedProd = CreateSketchStatus("DeployedProd", "Deployed to prod");
            SketchStatus skSentToSketch = CreateSketchStatus("SentToSketch", "Sent To Sketch");
            SketchStatus skAborted = CreateSketchStatus("Aborted", "Abort workflow deployment");

            context.SketchStatuses.AddOrUpdate(
                sk => sk.Status,
                skAborted,
                skDeployedDev,
                skDeployedProd,
                skSaved,
                skSentToSketch
                );

            WorkflowCode wc1 = CreateWorkflowCode("SampleWf1", "this is a sample wf code for testing (1)");
            WorkflowCode wc2 = CreateWorkflowCode("SampleWf2", "this is a sample wf code for testing (2)");
            WorkflowCode wc3 = CreateWorkflowCode("SampleWf3", "this is a sample wf code for testing (3)");
            WorkflowCode wc4 = CreateWorkflowCode("SampleWf4", "this is a sample wf code for testing (4)");
            WorkflowCode wc4a = CreateWorkflowCode("SampleWf4a", "this is a sample wf code for testing (4a)");
            WorkflowCode wc5 = CreateWorkflowCode("SampleWf5", "this is a sample wf code for testing (5)");
            WorkflowCode wc6 = CreateWorkflowCode("SampleWf6", "this is a sample wf code for testing (6)");
            WorkflowCode wc7 = CreateWorkflowCode("SampleWf7", "this is a sample wf code for testing (7)");
            WorkflowCode wc8 = CreateWorkflowCode("SampleWf8", "this is a sample wf code for testing (8)");
            WorkflowCode wc9 = CreateWorkflowCode("SampleWf9", "this is a sample wf code for testing (9)");
            WorkflowCode wcSketch = CreateWorkflowCode("SketchWf", "Sketch workflow code");
            WorkflowCode wcHoliday = CreateWorkflowCode("HolidayWf", "Holiday workflow code");

            context.WorkflowCodes.AddOrUpdate(
                wc => wc.Code,
                wc1, wc2, wc3, wc4, wc4a, wc5, wc6, wc7, wc8, wc9, wcSketch, wcHoliday);

            WorkflowConfiguration wfc1 = CreateWorkflowConfiguration(wc1, "", "", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc2 = CreateWorkflowConfiguration(wc2, "BasicHttpBinding_IFlowTasksOperations2", "http://localhost/Flow.Tasks.Workflows/SampleWf2.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc3 = CreateWorkflowConfiguration(wc3, "BasicHttpBinding_IFlowTasksOperations3", "http://localhost/Flow.Tasks.Workflows/SampleWf3.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc4 = CreateWorkflowConfiguration(wc4, "BasicHttpBinding_IFlowTasksOperations4", "http://localhost/Flow.Tasks.Workflows/SampleWf4.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc4a = CreateWorkflowConfiguration(wc4a, "BasicHttpBinding_IFlowTasksOperations4a", "http://localhost/Flow.Tasks.Workflows/SampleWf4a.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc5 = CreateWorkflowConfiguration(wc5, "BasicHttpBinding_IFlowTasksOperations5", "http://localhost/Flow.Tasks.Workflows/SampleWf5.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc6 = CreateWorkflowConfiguration(wc6, "BasicHttpBinding_IFlowTasksOperations6", "http://localhost/Flow.Tasks.Workflows/SampleWf6.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc7 = CreateWorkflowConfiguration(wc7, "BasicHttpBinding_IFlowTasksOperations7", "http://localhost/Flow.Tasks.Workflows/SampleWf7.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc8 = CreateWorkflowConfiguration(wc8, "BasicHttpBinding_IFlowTasksOperations8", "http://localhost/ServiceWorkflowsVB/SampleWf8.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfc9 = CreateWorkflowConfiguration(wc9, "BasicHttpBinding_IFlowTasksOperations9", "http://localhost/ServiceWorkflowsVB/SampleWf9.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfcSketch = CreateWorkflowConfiguration(wcSketch, "", "http://localhost/Flow.Tasks.Workflows/SketchWf.xamlx", "BasicHttpBinding_FlowTasks");
            WorkflowConfiguration wfcHoliday = CreateWorkflowConfiguration(wcHoliday, "", "http://localhost/Flow.Tasks.Workflows/HolidayWf.xamlx", "BasicHttpBinding_FlowTasks");

            context.WorkflowConfigurations.AddOrUpdate(
                wfc => wfc.ServiceUrl,
                wfc1, wfc2, wfc3, wfc4, wfc4a, wfc5, wfc6, wfc7, wfc8, wfc9, wfcSketch, wfcHoliday);

            // Topic
            TopicStatus topicNew = CreateTopicStatus("New", "New topic message");
            TopicStatus topicRead = CreateTopicStatus("Read", "Read topic message");

            context.TopicStatuses.AddOrUpdate(
                t => t.Status,
                topicNew,
                topicRead
                );

            // Holiday
            HolidayType pubType = CreateHolidayType("Public", "Public holiday");
            HolidayType anualType = CreateHolidayType("Annual", "Annual leave");
            HolidayType sickType = CreateHolidayType("Sick", "Sick leave");

            context.HolidayTypes.AddOrUpdate(
                h => h.Type,
                pubType, anualType, sickType
                );

            Holiday h2014 = CreateHoliday(pubType, 2014, HolidayStatus.A.ToString(), "01/01/2014,27/01/2014,25/04/2014,09/06/2014,06/10/2014,25/12/2014,26/12/2014");
            Holiday h2015 = CreateHoliday(pubType, 2015, HolidayStatus.A.ToString(), "01/01/2015,26/01/2015,25/04/2015,08/06/2015,05/10/2015,25/12/2015,28/12/2015");
            Holiday h2016 = CreateHoliday(pubType, 2016, HolidayStatus.A.ToString(), "01/01/2016,26/01/2016,25/04/2016,13/06/2016,03/10/2016,26/12/2016,27/12/2016");

            context.Holidays.AddOrUpdate(
                h => h.Year,
                h2014, h2015, h2016
                );

            // Sketch properties
            var sketchUrl = CreateProperty("SketchWorkflowUrl", "http://localhost/Flow.Tasks.Workflows/");
            var sketchPath = CreateProperty("SketchWorkflowPath", @"C:\Dev\Codeplex\FlowTasks\src\Workflows\ServiceWorkflows\");

            context.Properties.AddOrUpdate(
                p => p.Name,
                sketchUrl, sketchPath
                );

            var sketchWfpUrl = CreateWorkflowProperty(sketchUrl, wcSketch);
            var sketchWfpPath = CreateWorkflowProperty(sketchPath, wcSketch);

            context.WorkflowProperties.AddOrUpdate(
                p => new { p.WorkflowCodeId, p.WorkflowPropertyId },
                sketchWfpUrl, sketchWfpPath
                );


            context.Database.ExecuteSqlCommand(REMOVE_PROC);
            context.Database.ExecuteSqlCommand(REPORT_TASK_TIME_PROC);
            context.Database.ExecuteSqlCommand(REPORT_USER_TASK_COUNT_PROC);
            context.Database.ExecuteSqlCommand(REPORT_USER_TASKS_PROC);
            context.Database.ExecuteSqlCommand(REPORT_WORKFLOW_TIME_PROC);
            context.Database.ExecuteSqlCommand(TOPIC_SEARCH_FUNC);


        }

        private WorkflowProperty CreateWorkflowProperty(Property property, WorkflowCode workflowCode)
        {
            if (property.PropertyId != 0 && workflowCode.WorkflowCodeId != 0)
            {
                return new WorkflowProperty { PropertyId = property.PropertyId, WorkflowCodeId = workflowCode.WorkflowCodeId };
            }
            return new WorkflowProperty { Property = property, WorkflowCode = workflowCode };
        }

        private HolidayType CreateHolidayType(string type, string desc)
        {
            return new HolidayType { Type = type, Description = desc };
        }

        private Holiday CreateHoliday(HolidayType type, int year, string status, string dates)
        {
            if (type.HolidayTypeId != 0)
            {
                return new Holiday { HolidayTypeId = type.HolidayTypeId, Year = year, Status = status, Dates = dates };
            }
            return new Holiday { HolidayType = type, Year = year, Status = status, Dates = dates };
        }

        private TopicStatus CreateTopicStatus(string status, string description)
        {
            return new TopicStatus { Status = status, Description = description };
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
            if (workflowCode.WorkflowCodeId != 0)
            {
                return new WorkflowConfiguration
                {
                    EffectiveDate = DateTime.Now,
                    ServiceUrl = serviceUrl,
                    ServiceEndpoint = serviceEndpoint,
                    BindingConfiguration = bindingConfiguration,
                    WorkflowCodeId = workflowCode.WorkflowCodeId
                };
            }

            return new WorkflowConfiguration
            {
                EffectiveDate = DateTime.Now,
                ServiceUrl = serviceUrl,
                ServiceEndpoint = serviceEndpoint,
                BindingConfiguration = bindingConfiguration,
                WorkflowCode = workflowCode
            };
        }
    }
}
