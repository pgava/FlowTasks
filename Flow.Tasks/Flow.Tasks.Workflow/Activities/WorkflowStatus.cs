using System;
using System.Collections.Generic;

namespace Flow.Tasks.Workflow.Activities
{
    
    [Serializable]
    public class WorkflowStatus
    {
        public const string Name = "WorkflowStatus";
        
        private Dictionary<string, TaskStatus> _tasks;
        private Dictionary<string, string> _parameters;
        public string Result { get; set; }
        
        public Dictionary<string, TaskStatus> Tasks 
        {
            get { return _tasks ?? (_tasks = new Dictionary<string, TaskStatus>()); }
            set { _tasks = value; }
        }
        public Dictionary<string, string> Parameters 
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
            set { _parameters = value; }
        }

        public void AddTask(string key, TaskStatus value) 
        { 
            if (string.IsNullOrEmpty(key))
                return;
            value.WorkflowStatusRef = this;
            Tasks.Add(key, value); 
        }

        public void AddParameter(string key, string value) 
        { 
            if (string.IsNullOrEmpty(key))                
                return;
            Parameters.Add(key, value); 
        }

    }
}
