using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Flow.Tasks.Web.Models
{

    /// <summary>
    /// Dashboard Workflow Model
    /// </summary>
    public class DashboardWorkflowsModel
    {
        public IEnumerable<DashboardWorkflowModel> Workflows { get; set; }

        public int TotalWorkflows { get; set; }
        public IEnumerable<string> WorkflowCodes { get; set; }
    
    }


    /// <summary>
    /// Dashboard Workflow Model
    /// </summary>
    public class DashboardWorkflowModel
    {
        public string wfid { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string status { get; set; }
        public string code { get; set; }
        public string select { get; set; }
        public string level { get; set; }
        public string parent { get; set; }
        public string parentid { get; set; }
        public string isLeaf { get; set; }
        public string expanded { get; set; }
        public string loaded { get; set; }

        public DashboardWorkflowFilterModel WorkflowFilter { get; set; }
    }

    /// <summary>
    /// Dashboard Workflow FilterModel
    /// </summary>
    public class DashboardWorkflowFilterModel
    {
        [Display(Name = "Workflow Id")]
        public string WorkflowId { get; set; }

        [Display(Name = "Workflow Code")]
        public string Code { get; set; }

        [Display(Name = "Domain")]
        public string Domain { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Started From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Start { get; set; }

        [Display(Name = "To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? End { get; set; }

        public string Actionid { get; set; }

        public string PageSize { get; set; }

        public IEnumerable<string> CodeList { get; set; }
        public IEnumerable<string> DomainList { get; set; }

        public IEnumerable<WorkflowCodes.WorkflowCodeItem> CodeItems
        {
            get
            {
                return new WorkflowCodes(CodeList).Items;
            }
        }

        public IEnumerable<Domains.DomainItem> DomainItems
        {
            get
            {
                return new Domains(DomainList).Items;
            }
        }

        public IEnumerable<DashboardWorkflowPageSize.PageSizeItem> PageSizeItems
        {
            get
            {
                return new DashboardWorkflowPageSize().Items;
            }
        }

        public static string GetDate(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("MM/dd/yyyy") : "";
        }

        static public DashboardWorkflowFilterModel Build(string workflowId, string code, string domain, string isActive, string start, string end, string actionId)
        {

            return new DashboardWorkflowFilterModel
            {
                WorkflowId = workflowId,
                Code = string.IsNullOrWhiteSpace(code) ? Library.Properties.Resources.SELECT_DEFAULT : code,
                Domain = string.IsNullOrWhiteSpace(domain) ? Library.Properties.Resources.SELECT_DEFAULT : domain,
                IsActive = string.IsNullOrWhiteSpace(isActive) ? true : bool.Parse(isActive),
                Start = string.IsNullOrWhiteSpace(start) ? (DateTime?)null : DateTime.Parse(start),
                End = string.IsNullOrWhiteSpace(end) ? (DateTime?)null : DateTime.Parse(end),
                Actionid = actionId
            };
        }
    }

    /// <summary>
    /// Workflow Codes
    /// </summary>
    public class WorkflowCodes
    {
        public readonly string All = Library.Properties.Resources.SELECT_DEFAULT;

        public class WorkflowCodeItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public WorkflowCodeItem(string text, string value)
            {
                Text = text; Value = value;
            }
        }

        public List<WorkflowCodeItem> Items { get; set; }

        public WorkflowCodes(IEnumerable<string> workflowcodes)
        {

            Items = new List<WorkflowCodeItem> { new WorkflowCodeItem(All, All) };

            if (workflowcodes == null) return;

            foreach (var d in workflowcodes)
            {
                Items.Add(new WorkflowCodeItem(d, d));
            }
        }
    }

    /// <summary>
    /// Dashboard Workflow PageSize
    /// </summary>
    public class DashboardWorkflowPageSize
    {
        public const string Size100 = "100";
        public const string Size500 = "500";
        public const string SizeAll = "All";

        public class PageSizeItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public PageSizeItem(string text, string value)
            {
                Text = text; Value = value;
            }
        }

        public List<PageSizeItem> Items { get; set; }

        public DashboardWorkflowPageSize()
        {

            Items = new List<PageSizeItem>
                        {
                            new PageSizeItem("100", Size100),
                            new PageSizeItem("500", Size500),
                            new PageSizeItem("All", SizeAll)
                        };
        }

    }

    /// <summary>
    /// Domains
    /// </summary>
    public class Domains
    {
        public readonly string All = Library.Properties.Resources.SELECT_DEFAULT;

        public class DomainItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public DomainItem(string text, string value)
            {
                Text = text; Value = value;
            }
        }

        public List<DomainItem> Items { get; set; }

        public Domains(IEnumerable<string> domains)
        {

            Items = new List<DomainItem> { new DomainItem(All, All) };

            if (domains == null) return;

            foreach (var d in domains)
            {
                Items.Add(new DomainItem(d, d));
            }
        }

    }

    /// <summary>
    /// Error Message
    /// </summary>
    public class ErrorMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Trace Item
    /// </summary>
    public class TraceItem
    {
        public string When { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Property Item
    /// </summary>
    public class PropertyItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    /// <summary>
    /// Trace Model
    /// </summary>
    public class TraceModel
    {
        public IEnumerable<TraceItem> Traces { get; set; }
    }

    /// <summary>
    /// Property Model
    /// </summary>
    public class PropertyModel
    {
        public IEnumerable<PropertyItem> Properties { get; set; }
    }

    /// <summary>
    /// Task Item
    /// </summary>
    public class TaskItem
    {
        public string TaskId { get; set; }
        public string Code { get; set; }
        public string ExpiryDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AcceptedBy { get; set; }
    }

    /// <summary>
    /// Task Model
    /// </summary>
    public class TaskModel
    {
        public IEnumerable<TaskItem> Tasks { get; set; }
    }

    /// <summary>
    /// Dashboard Show Workflow Model
    /// </summary>
    public class DashboardShowWorkflowModel
    {
        public TaskModel Tasks { get; set; }
        public PropertyModel Properties { get; set; }
        public TraceModel Traces { get; set; }

    }
}