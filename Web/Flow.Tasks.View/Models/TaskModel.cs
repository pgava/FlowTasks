using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.View.Models
{
    /// <summary>
    /// TaskModel
    /// </summary>
    public class TaskModel 
    {
        [Display(Name = "Task UI")]
        public string UiCode { get; set; }

        [Display(Name = "Task Code")]
        public string TaskCode { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Default Result")]
        public string DefaultResult { get; set; }

        [Display(Name = "Task Oid")]
        public string TaskOid { get; set; }

        [Display(Name = "Workflow Oid")]
        public string WorkflowOid { get; set; }

        [Display(Name = "Task Correlation Id")]
        public int TaskCorrelationId { get; set; }

        [Display(Name = "Expires")]
        public string Expires { get; set; }

        [Display(Name = "Task Assigned")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Task Has Document")]
        public bool HasDocument { get; set; }

        /*
         * If you add an object here, you have to manualy change also taskimpl.js
         */
        public DocumentModel[] Documents { get; set; }

        public FilterModel Filter { get; set; }

        public CommentModel Comment { get; set; }

        public PropertyInfo[] Parameters { get; set; }

        public string GetPropertyValueFromName(string name)
        {
            foreach (var prop in Parameters)
            {
                if (prop.Name == name) return prop.Value;
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// TaskListModel
    /// </summary>
    public class TaskListModel 
    {
        public List<TaskModel> Tasks { get; set; }
        public TaskModel TaskSelected { get; set; }
    }

    /// <summary>
    /// OrderListBy
    /// </summary>
    public class OrderListBy
    {
        public const string TaskName = "TaskName";
        public const string ExpiryDateAsc = "ExpiryDateAsc";
        public const string ExpiryDateDesc = "ExpiryDateDesc";

        public class OrderListItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public OrderListItem(string text, string value)
            {
                Text = text; Value = value;
            }
        }

        public List<OrderListItem> Items { get; set; }

        public OrderListBy()
        {

            Items = new List<OrderListItem>
                        {
                            new OrderListItem("Task Name", TaskName),
                            new OrderListItem("Expiry Date Asc", ExpiryDateAsc),
                            new OrderListItem("Expiry Date Desc", ExpiryDateDesc)
                        };
        }

    }

    /// <summary>
    /// MaxTasks
    /// </summary>
    public class MaxTasks
    {
        public const string Tasks5 = "5";
        public const string Tasks10 = "10";
        public const string Tasks15 = "15";

        public class MaxTasksItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public MaxTasksItem(string text, string value)
            {
                Text = text; Value = value;
            }
        }

        public List<MaxTasksItem> Items { get; set; }

        public MaxTasks()
        {
            Items = new List<MaxTasksItem>
                {
                    new MaxTasksItem(Tasks5.ToString(CultureInfo.InvariantCulture), Tasks5),
                    new MaxTasksItem(Tasks10.ToString(CultureInfo.InvariantCulture), Tasks10),
                    new MaxTasksItem(Tasks15.ToString(CultureInfo.InvariantCulture), Tasks15)
                };
        }

    }

    /// <summary>
    /// Domains
    /// </summary>
    public class Domains
    {
        public const string All = "All";

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

        public Domains(IEnumerable<string>domains)
        {

            Items = new List<DomainItem> {new DomainItem(All, All)};

            if (domains == null) return;

            foreach (var d in domains)
            {
                Items.Add(new DomainItem(d, d));
            }            
        }

    }

    /// <summary>
    /// DisplayBy
    /// </summary>
    public class DisplayBy
    {
        public const string All = "All";
        public const string DueToday = "DueToday";
        public const string DueTomorrow = "DueTomorrow";
        public const string Overdue = "Overdue";
        public const string None = "None";
    }

    /// <summary>
    /// CommentItem
    /// </summary>
    public class CommentItem
    {
        public string When { get; set; }
        public string User { get; set; }
        public string Avatar { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// CommentModel
    /// </summary>
    public class CommentModel
    {
        public enum CommentStatus
        {
            Disabled,
            Mandatory,
            Optional
        }

        public string Status { get; set; }
        public string TaskComment { get; set; }
        public IEnumerable<CommentItem> Comments { get; set; }
    }

    /// <summary>
    /// FilterModel
    /// </summary>
    public class FilterModel
    {
        public string OrderMethod { get; set; }
        public string DisplayMethod { get; set; }
        public string Filter { get; set; }
        public string CurrentPage { get; set; }
        public string FilteredPages { get; set; }
        public string TotalPages { get; set; }
        public string TotalTasks { get; set; }
        public string Domain { get; set; }
        public string MaxTasks { get; set; }
        public IEnumerable<string> DomainList { get; set; }

        public string NavPosition
        {
            get
            {
                return "Showing page " + CurrentPage + " of " + FilteredPages + " (Tot: " + TotalTasks + ")";
            }
        }
        public IEnumerable<OrderListBy.OrderListItem> OrderItems
        {
            get
            {
                return new OrderListBy().Items;
            }
        }
        public IEnumerable<Domains.DomainItem> DomainItems
        {
            get
            {
                return new Domains(DomainList).Items;
            }
        }
        public IEnumerable<MaxTasks.MaxTasksItem> MaxTasksItems
        {
            get
            {
                return new MaxTasks().Items;
            }
        }
    }

    /// <summary>
    /// DocumentModel
    /// </summary>
    public class DocumentModel
    {
        public string DocumentOid { get; set; }
        public string DocumentName { get; set; }
    }
}