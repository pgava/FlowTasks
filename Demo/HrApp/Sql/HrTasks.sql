use FlowTasks
go

exec sp_executesql N'insert [dbo].[WorkflowCode]([Code], [Description])
values (@0, @1)
select [WorkflowCodeId]
from [dbo].[WorkflowCode]
where @@ROWCOUNT > 0 and [WorkflowCodeId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ',@0=N'HrProcess',@1=N'this is a hiring process workflow'

declare @wc int

select @wc = (
	select WorkflowCodeId
	from WorkflowCode
	where Code = 'HrProcess'
)

exec sp_executesql N'insert [dbo].[WorkflowConfiguration]([ServiceDefinition], [ServiceUrl], [BindingConfiguration], [ServiceEndpoint], [EffectiveDate], [ExpiryDate], [WorkflowCodeId])
values (null, @0, @1, @2, @3, null, @4)
select [WorkflowConfigurationId]
from [dbo].[WorkflowConfiguration]
where @@ROWCOUNT > 0 and [WorkflowConfigurationId] = scope_identity()',N'@0 nvarchar(max) ,@1 nvarchar(max) ,@2 nvarchar(max) ,@3 datetime2(7),@4 int',@0=N'http://localhost/Flow.Tasks.Workflows/HrProcess.xamlx',@1=N'BasicHttpBinding_FlowTasks',@2=N'BasicHttpBinding_IFlowTasksOperationsControlHr',@3='2012-12-05 08:26:19.3638340',@4=@wc
go
