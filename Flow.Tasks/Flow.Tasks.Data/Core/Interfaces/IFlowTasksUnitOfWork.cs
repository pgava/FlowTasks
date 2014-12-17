using Flow.Library.Interfaces;

namespace Flow.Tasks.Data.Core.Interfaces
{
    public interface IFlowTasksUnitOfWork : IUnitOfWork
    {
        IRepository<WorkflowCode> WorkflowCodes { get; }
        IRepository<WorkflowConfiguration> WorkflowConfigurations { get; }
        IRepository<TaskConfiguration> TaskConfigurations { get; }
        IRepository<WorkflowProperty> WorkflowProperties { get; }
        IRepository<TaskProperty> TaskProperties { get; }
        IRepository<WorkflowDefinition> WorkflowDefinitions { get; }
        IRepository<TaskDefinition> TaskDefinitions { get; }
        IRepository<Property> Properties { get; }
        IRepository<TaskInParameter> TaskInParameters { get; }
        IRepository<WorkflowInParameter> WorkflowInParameters { get; }
        IRepository<TaskOutParameter> TaskOutParameters { get; }
        IRepository<WorkflowOutParameter> WorkflowOutParameters { get; }
        IRepository<TraceEvent> TraceEvents { get; }
        IRepository<WorkflowTrace> WorkflowTraces { get; }
        IRepository<TaskUser> TaskUsers { get; }
        IRepository<TaskUserHandOver> TaskUserHandOvers { get; }
        IRepository<WorkflowStatus> WorkflowStatuses { get; }
        IRepository<Holiday> Holidays { get; }
        IRepository<SketchConfiguration> SketchConfigurations { get; }
        IRepository<SketchStatus> SketchStatuses { get; }
        IRepository<Topic> Topics { get; }
        IRepository<TopicAttachment> TopicAttachments { get; }
        IRepository<TopicMessage> TopicMessages { get; }
        IRepository<TopicStatus> TopicStatuses { get; }
        IRepository<TopicUser> TopicUsers { get; }
    }
}
