using System;
using System.Collections.Generic;

namespace Flow.Tasks.Workflow.Activities
{
    
    [Serializable]
    public class WorkflowStateData
    {
        public const string Name = "WorkflowStateData";
        
        private Dictionary<string, TaskStateData> _tasks;
        private Dictionary<string, string> _parameters;
        public string Result { get; set; }
        
        public Dictionary<string, TaskStateData> Tasks 
        {
            get { return _tasks ?? (_tasks = new Dictionary<string, TaskStateData>()); }
            set { _tasks = value; }
        }
        public Dictionary<string, string> Parameters 
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
            set { _parameters = value; }
        }

        public void AddTask(string key, TaskStateData value) 
        { 
            if (string.IsNullOrEmpty(key))
                return;
            Tasks.Add(key, value); 
        }

        public void AddParameter(string key, string value) 
        { 
            if (string.IsNullOrEmpty(key))                
                return;
            Parameters.Add(key, value); 
        }

        //public static WorkflowStateData GetWorkflowState(NativeActivityContext context)
        //{
        //    var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            
        //    if (workflowStatus == null)
        //    {
        //        workflowStatus = context.GetExtension<WorkflowStateData>();
        //    }

        //    if (workflowStatus == null)
        //    {
        //        throw new Exception("Cannot find WorkflowStateData");
        //    }

        //    return workflowStatus;
        //}
    }
}
