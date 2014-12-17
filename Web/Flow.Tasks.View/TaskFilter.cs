using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Models;

namespace Flow.Tasks.View
{
    /// <summary>
    /// Task Filter
    /// </summary>
    public class TaskFilter
    {
        /// <summary>
        /// Filter Model
        /// </summary>
        FilterModel _model;

        public TaskFilter(FilterModel model)
        {
            _model = model;
        }

        /// <summary>
        /// Get Page Size
        /// </summary>
        /// <returns>Page size</returns>
        private int GetPageSize()
        {
            if (_model == null || _model.MaxTasks == null || string.IsNullOrWhiteSpace(_model.MaxTasks))
                return int.Parse(MaxTasks.Tasks10);

            return int.Parse(_model.MaxTasks);
        }

        /// <summary>
        /// Order Tasks
        /// </summary>
        /// <param name="tasks">Tasks</param>
        /// <returns>List of ordered TaskInfo</returns>
        private IEnumerable<TaskInfo> OrderTasks(IEnumerable<TaskInfo> tasks)
        {
            if (_model.OrderMethod == OrderListBy.TaskName)
            {
                return tasks.OrderBy(t => t.Title);
            }
            if (_model.OrderMethod == OrderListBy.ExpiryDateAsc)
            {
                return tasks.OrderBy(t => (!t.ExpiryDate.HasValue) ? DateTime.MaxValue : t.ExpiryDate);
            }
            if (_model.OrderMethod == OrderListBy.ExpiryDateDesc)
            {
                return tasks.OrderByDescending(t => (!t.ExpiryDate.HasValue) ? DateTime.MaxValue : t.ExpiryDate);
            }

            return tasks;
        }

        /// <summary>
        /// Match filter
        /// </summary>
        /// <param name="t">TaskInfo</param>
        /// <returns>True or False</returns>
        private bool IsSearch(TaskInfo t)
        {
            if (!string.IsNullOrWhiteSpace(_model.Filter))
            {
                if (t.Title.IndexOf(_model.Filter, StringComparison.Ordinal) == -1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Match display method
        /// </summary>
        /// <param name="t">TaskInfo</param>
        /// <returns>True or False</returns>
        private bool IsDisplay(TaskInfo t)
        {
            if (!string.IsNullOrWhiteSpace(_model.DisplayMethod) && _model.DisplayMethod != DisplayBy.All)
            {
                if (t.ExpiryDate.HasValue)
                {
                    if (_model.DisplayMethod == DisplayBy.DueToday)
                    {
                        if (!(t.ExpiryDate.Value.Date == DateTime.Now.Date))
                        {
                            return false;
                        }
                    }
                    if (_model.DisplayMethod == DisplayBy.DueTomorrow)
                    {
                        if (!(t.ExpiryDate.Value.Date == DateTime.Now.Date.AddDays(1)))
                        {
                            return false;
                        }
                    }
                    if (_model.DisplayMethod == DisplayBy.Overdue)
                    {
                        if (!(t.ExpiryDate.Value.Date < DateTime.Now.Date))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (_model.DisplayMethod != DisplayBy.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Run Search
        /// </summary>
        /// <param name="tasks">List of TaskInfo</param>
        /// <returns>Filtered list of TaskInfo</returns>
        public IEnumerable<TaskInfo> RunSearch(IEnumerable<TaskInfo> tasks)
        {
            var trimedTasks = new List<TaskInfo>(GetPageSize());
            var taskCounter = 0;

            int page;
            var taskInfos = tasks as IList<TaskInfo> ?? tasks.ToList();
            var taskStartIdx = GetStartIndex(taskInfos, out page);
            var idx = 0;

            tasks = OrderTasks(taskInfos);

            foreach (var t in tasks)
            {
                if (!IsSearch(t)) continue;

                if (!IsDisplay(t)) continue;

                idx++;

                if (taskCounter >= GetPageSize()) continue;

                if (idx > taskStartIdx)
                {
                    taskCounter++;
                    trimedTasks.Add(t);
                }
            }

            _model.TotalTasks = tasks.Count().ToString(CultureInfo.InvariantCulture);
            _model.CurrentPage = page.ToString(CultureInfo.InvariantCulture);
            _model.FilteredPages = Math.Ceiling((double)idx / GetPageSize()).ToString(CultureInfo.InvariantCulture);
            _model.TotalPages = Math.Ceiling((double)tasks.Count() / GetPageSize()).ToString(CultureInfo.InvariantCulture);

            return trimedTasks;
        }

        /// <summary>
        /// Get Start Index
        /// </summary>
        /// <param name="tasks">Tasks</param>
        /// <param name="page">Page</param>
        /// <returns>index</returns>
        private int GetStartIndex(IEnumerable<TaskInfo> tasks, out int page)
        {
            if (!int.TryParse(_model.CurrentPage, out page))
            {
                page = 1;
            }

            var taskStartIdx = (page - 1) * GetPageSize();

            if (tasks.Count() <= taskStartIdx && page > 1)
            {
                page -= 1;
                taskStartIdx = (page - 1) * GetPageSize();
            }

            return taskStartIdx;
        }

        /// <summary>
        /// Get Filter Model
        /// </summary>
        /// <returns>FilterModel</returns>
        public FilterModel GetFilterModel()
        {
            return _model;
        }
    }
}