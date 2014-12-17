using System.Collections.Generic;
using System;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Workflow.Activities
{
    [Serializable]
    public sealed class TaskStateData
    {
        private Dictionary<string, string> _parameters;

        public TaskInfo TaskInfo { get; set; }
        public string AssignedUser { get; set; }
        public string Result { get; set; }

        public Dictionary<string, string> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
            set { _parameters = value; }
        }

        public void AddParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            
            if (Parameters.ContainsKey(key)) Parameters[key] = value;
            else Parameters.Add(key, value);
        }
    }

}
