using Flow.Library.EF;
using Flow.Tasks.Data.Core.Interfaces;
using Flow.Library;
using Flow.Tasks.Data.Core;
using Flow.Library.Interfaces;
using System;

namespace Flow.Tasks.Data.Infrastructure
{
    public class TasksObjectSetFactoryAttribute : Attribute { }
    
    public sealed class FlowTasksUnitOfWork : UnitOfWork, IFlowTasksUnitOfWork
    {
        Repository<WorkflowCode> _workflowCodes;
        Repository<WorkflowConfiguration> _workflowConfigurations;
        Repository<TaskConfiguration> _taskConfigurations;
        Repository<WorkflowProperty> _workflowProperties;
        Repository<TaskProperty> _taskProperties;
        Repository<WorkflowDefinition> _workflowDefinitions;
        Repository<TaskDefinition> _taskDefinitions;
        Repository<Property> _properties;
        Repository<TaskInParameter> _taskInParameters;
        Repository<WorkflowInParameter> _workflowInParameters;
        Repository<TaskOutParameter> _taskOutParameters;
        Repository<WorkflowOutParameter> _workflowOutParameters;
        Repository<TraceEvent> _traceEvents;
        Repository<WorkflowTrace> _workflowTraces;
        Repository<TaskUser> _taskUsers;
        Repository<TaskUserHandOver> _taskUserHandOvers;
        Repository<WorkflowStatus> _workflowStatuses;
        Repository<SketchStatus> _sketchStatuses;
        Repository<SketchConfiguration> _sketchConfigurations;
        Repository<Topic> _topics;
        Repository<TopicMessage> _topicMessages;
        Repository<TopicUser> _topicUsers;
        Repository<TopicAttachment> _topicAttachments;
        Repository<TopicStatus> _topicStatuses;
        Repository<Holiday> _holidays;
        Repository<HolidayType> _holidayTypes;

        readonly IObjectSetFactory _contextAdapter;

        public FlowTasksUnitOfWork([TasksObjectSetFactory]IObjectSetFactory contextAdapter)
            : base(contextAdapter)
        {
            _contextAdapter = contextAdapter;
        }

        public FlowTasksUnitOfWork()
        {
            _contextAdapter = new FlowTasksContextAdapter(new FlowTasksEntities());
            ObjectContext = _contextAdapter;
        }

        #region IFlowTasksUnitOfWork Members

        public IRepository<WorkflowCode> WorkflowCodes
        {
            get { return _workflowCodes ?? (_workflowCodes = new Repository<WorkflowCode>(_contextAdapter)); }
        }

        public IRepository<WorkflowConfiguration> WorkflowConfigurations
        {
            get {
                return _workflowConfigurations ??
                       (_workflowConfigurations = new Repository<WorkflowConfiguration>(_contextAdapter));
            }
        }

        public IRepository<TaskConfiguration> TaskConfigurations
        {
            get { return _taskConfigurations ?? (_taskConfigurations = new Repository<TaskConfiguration>(_contextAdapter)); }
        }

        public IRepository<WorkflowProperty> WorkflowProperties
        {
            get { return _workflowProperties ?? (_workflowProperties = new Repository<WorkflowProperty>(_contextAdapter)); }
        }

        public IRepository<TaskProperty> TaskProperties
        {
            get { return _taskProperties ?? (_taskProperties = new Repository<TaskProperty>(_contextAdapter)); }
        }

        public IRepository<WorkflowDefinition> WorkflowDefinitions
        {
            get {
                return _workflowDefinitions ??
                       (_workflowDefinitions = new Repository<WorkflowDefinition>(_contextAdapter));
            }
        }

        public IRepository<TaskDefinition> TaskDefinitions
        {
            get { return _taskDefinitions ?? (_taskDefinitions = new Repository<TaskDefinition>(_contextAdapter)); }
        }

        public IRepository<Property> Properties
        {
            get { return _properties ?? (_properties = new Repository<Property>(_contextAdapter)); }
        }

        public IRepository<TaskInParameter> TaskInParameters
        {
            get { return _taskInParameters ?? (_taskInParameters = new Repository<TaskInParameter>(_contextAdapter)); }
        }

        public IRepository<WorkflowInParameter> WorkflowInParameters
        {
            get {
                return _workflowInParameters ??
                       (_workflowInParameters = new Repository<WorkflowInParameter>(_contextAdapter));
            }
        }

        public IRepository<TaskOutParameter> TaskOutParameters
        {
            get { return _taskOutParameters ?? (_taskOutParameters = new Repository<TaskOutParameter>(_contextAdapter)); }
        }

        public IRepository<WorkflowOutParameter> WorkflowOutParameters
        {
            get {
                return _workflowOutParameters ??
                       (_workflowOutParameters = new Repository<WorkflowOutParameter>(_contextAdapter));
            }
        }

        public IRepository<TraceEvent> TraceEvents
        {
            get { return _traceEvents ?? (_traceEvents = new Repository<TraceEvent>(_contextAdapter)); }
        }

        public IRepository<WorkflowTrace> WorkflowTraces
        {
            get { return _workflowTraces ?? (_workflowTraces = new Repository<WorkflowTrace>(_contextAdapter)); }
        }

        public IRepository<TaskUser> TaskUsers
        {
            get { return _taskUsers ?? (_taskUsers = new Repository<TaskUser>(_contextAdapter)); }
        }

        public IRepository<TaskUserHandOver> TaskUserHandOvers
        {
            get { return _taskUserHandOvers ?? (_taskUserHandOvers = new Repository<TaskUserHandOver>(_contextAdapter)); }
        }

        public IRepository<WorkflowStatus> WorkflowStatuses
        {
            get { return _workflowStatuses ?? (_workflowStatuses = new Repository<WorkflowStatus>(_contextAdapter)); }
        }

        public IRepository<SketchStatus> SketchStatuses
        {
            get { return _sketchStatuses ?? (_sketchStatuses = new Repository<SketchStatus>(_contextAdapter)); }
        }
        
        public IRepository<SketchConfiguration> SketchConfigurations
        {
            get { return _sketchConfigurations ?? (_sketchConfigurations = new Repository<SketchConfiguration>(_contextAdapter)); }
        }

        public IRepository<Topic> Topics
        {
            get { return _topics ?? (_topics = new Repository<Topic>(_contextAdapter)); }
        }

        public IRepository<TopicMessage> TopicMessages
        {
            get { return _topicMessages ?? (_topicMessages = new Repository<TopicMessage>(_contextAdapter)); }
        }

        public IRepository<TopicUser> TopicUsers
        {
            get { return _topicUsers ?? (_topicUsers = new Repository<TopicUser>(_contextAdapter)); }
        }

        public IRepository<TopicAttachment> TopicAttachments
        {
            get { return _topicAttachments ?? (_topicAttachments = new Repository<TopicAttachment>(_contextAdapter)); }
        }

        public IRepository<TopicStatus> TopicStatuses
        {
            get { return _topicStatuses ?? (_topicStatuses = new Repository<TopicStatus>(_contextAdapter)); }
        }

        public IRepository<Holiday> Holidays
        {
            get { return _holidays ?? (_holidays = new Repository<Holiday>(_contextAdapter)); }
        }

        public IRepository<HolidayType> HolidayTypes
        {
            get { return _holidayTypes ?? (_holidayTypes = new Repository<HolidayType>(_contextAdapter)); }
        }

        #endregion
    }
}
