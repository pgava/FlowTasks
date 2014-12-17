flowTasksApp
.directive('loadReport', ['ApplicationFactory', function (applicationFactory) {
    return {
        restrict: "A",
        link: function (scope, elem) {
            applicationFactory.preLoading.show(false);
        }
    }
}])

// User Tasks
.directive('reportUserTasks', ['globalConfig', 'ReportService','myAlert', function (globalConfig, reportService,myAlert) {
    return {
        restrict: "A",
        templateUrl: 'scripts/flowTasksApp/views/report/index.usertasks.html',
        scope: true,
        link: function (scope, elem, attr) {
            // Set start date default value is last month
            scope.startDate = moment().add(-1, 'months').format(globalConfig.momentDateFormat);
            // Set end date default value is current month
            scope.endDate = moment().format(globalConfig.momentDateFormat);
            // User list
            scope.selectedUsersList = [];
            scope.usersList = [];
            // Datetime format
            scope.dateFormat = globalConfig.dateFormat;

            // Refresh
            scope.refresh = function() {
                rptUserTasks();
            };

            //-------------------------------------------------------------------------
            // Report User Tasks - callback
            // Callback that creates and populates a data table,
            // instantiates the pie chart, passes in the data and
            // draws it.
            //-------------------------------------------------------------------------
            var rptUserTasks = function () {

                // Ajax call to get report data and draw report
                ajaxCallUserTasks();
            }

            //-------------------------------------------------------------------------
            // Get User Tasks Filter
            //-------------------------------------------------------------------------
            var getUserTasksFilter = function () {
                return {
                    Start: scope.startDate,
                    End: scope.endDate,
                    U: scope.selectedUsersList.length > 0 ? scope.selectedUsersList : null
                };
            };

            //-------------------------------------------------------------------------
            //Ajax Call User Tasks - Get report data
            //-------------------------------------------------------------------------
            var ajaxCallUserTasks = function () {

                var filter = getUserTasksFilter();
                var json = filter;

                reportService.getUserTasks(json).then(function (data) {
                    // Success goes here

                    // Clear user list in multiselect
                    scope.usersList = [];

                    // Initial google chart
                    scope.chartObject = {
                        options: {
                            title: 'Total number of tasks completed by user',
                            titleTextStyle: { fontSize: "18" }
                        }
                    };

                    scope.chartObject.data = {
                        'cols': [
                            { id: "u", label: "User", type: "string" },
                            { id: "t", label: "Tasks", type: "number" }
                        ],
                        'rows': []
                    };
                    scope.chartObject.type = "PieChart";

                    if (data.length !== 0) {
                        angular.forEach(data, function (rptRow, idx) {
                            // Push data to chart
                            scope.chartObject.data.rows.push({
                                c: [
                                    { v: rptRow.user },
                                    { v: rptRow.taskNo }
                                ]
                            });

                            // Push user to user list
                            scope.usersList.push(rptRow.user);
                        });
                    }

                    // clear selected users after complete
                    scope.selectedUsersList = [];
                },
                function (err) {
                    // Error goes here
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading user tasks report');
                });

            }
            
            // Load reporter
            rptUserTasks();
        }
    }
}])

// User Task Count
.directive('reportUserTaskCount', ['globalConfig', '$filter', 'ReportService','myAlert', function (globalConfig, $filter, reportService,myAlert) {
    return {
        restrict: "A",
        templateUrl: 'scripts/flowTasksApp/views/report/index.usertaskcount.html',
        scope: true,
        link: function (scope, elem) {

            // Callback that creates and populates a data table,
            // instantiates the column chart, passes in the data and
            // draws it.
            //-------------------------------------------------------------------------
            var rptUserTaskCount = function () {
                // Ajax call to get report data and draw report
                ajaxCallUserTaskCount();
            }

            //-------------------------------------------------------------------------
            // Get User Task Count Filter
            //-------------------------------------------------------------------------
            var getUserTaskCountFilter = function () {
                return {
                    Start: scope.startDate,
                    End: scope.endDate,
                    U: scope.selectedUsersList.length > 0 ? scope.selectedUsersList : null,
                    T: scope.selectedTasksList.length > 0 ? scope.selectedTasksList : null
                };
            };

            //-------------------------------------------------------------------------
            // Ajax Call - Get report data
            // We need to change the data table from 
            //
            // User | Task | Duration
            // uuu  | ttt  | 2
            // vvv  | ttt  | 1
            // uuu  | rrr  | 4
            // vvv  | rrr  | 3
            //
            // To
            // 
            // Task | uuu | vvv 
            // ttt  | 2   | 1
            // rrr  | 4   | 3
            //
            //-------------------------------------------------------------------------
            var ajaxCallUserTaskCount = function () {

                var filter = getUserTaskCountFilter();
                var json = filter;

                reportService.getUserTaskCount(json).then(function (data) {
                    // Success goes here
                    scope.usersList = [];
                    scope.tasksList = [];

                    // Initial google chart
                    scope.chartObject = {
                        options: {
                            title: 'Tasks completed by user',
                            titleTextStyle: { fontSize: "18" }
                        }
                    };

                    scope.chartObject.data = {
                        'cols': [
                            { id: "t", label: "Task", type: "string" }
                        ],
                        'rows': []
                    };

                    scope.chartObject.type = "ColumnChart";

                    if (data.length !== 0) {
                        var users = [];
                        var tasks = [];

                        var usersStr = "";
                        var tasksStr = "";

                        // Get users/tasks
                        angular.forEach(data, function (rptRow) {
                            if (!usersStr.match(rptRow.user)) {
                                users.push(rptRow.user);
                                usersStr += "*-*" + rptRow.user;
                                scope.usersList.push(rptRow.user);
                            }
                            if (!tasksStr.match(rptRow.task)) {
                                tasks.push(rptRow.task);
                                tasksStr += "*-*" + rptRow.task;
                                scope.tasksList.push(rptRow.task);
                            }
                        });

                        // Add columns for every user
                        angular.forEach(users, function (u) {
                            scope.chartObject.data.cols.push(
                                { id: "u", label: u, type: "number" }
                            );
                        });

                        usersStr = "";
                        tasksStr = "";

                        // Add Data to reporter
                        angular.forEach(tasks, function (t) {
                            // define row object
                            var rowObj = {
                                c: [
                                    { v: t }
                                ]
                            }

                            angular.forEach(users, function (u3) {
                                var dataFound = $filter("getReportDataByUserAndTask")(data, u3, t);
                                var count = 0;
                                if (dataFound != null) {
                                    count = dataFound.count;
                                }
                                rowObj.c.push({ v: count });
                            });

                            // push row
                            scope.chartObject.data.rows.push(rowObj);
                        });

                        // Refresh selected values
                        scope.selectedUsersList = [];
                        scope.selectedTasksList = [];
                    }
                }, function (err) {
                    // Error goes here
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading user task count report');
                });

            }

            // Set start date default value is last month
            scope.startDate = moment().add(-1, 'months').format(globalConfig.momentDateFormat);
            // Set end date default value is current month
            scope.endDate = moment().format(globalConfig.momentDateFormat);
            // Datetime format
            scope.dateFormat = globalConfig.dateFormat;

            // Users list
            scope.selectedUsersList = [];
            scope.usersList = [];

            //Tasks list
            scope.selectedTasksList = [];
            scope.tasksList = [];

            // Refresh
            scope.refresh = function () {
                rptUserTaskCount();
            };
            
            // Load reporter
            rptUserTaskCount();
        }
    }
}])

// Task
.directive('reportTask', ['globalConfig', 'ReportService','myAlert', function (globalConfig, reportService,myAlert) {
    return {
        restrict: "A",
        templateUrl: 'scripts/flowTasksApp/views/report/index.task.html',
        scope: true,
        link: function (scope, elem) {

            //-------------------------------------------------------------------------
            // Report Task Time - callback
            // Callback that creates and populates a data table,
            // instantiates the pie chart, passes in the data and
            // draws it.
            //-------------------------------------------------------------------------
            var rptTaskTime = function () {

                // Ajax call to get report data and draw report
                ajaxCallTaskTime();

            }

            //-------------------------------------------------------------------------
            // Get Filter
            //-------------------------------------------------------------------------
            var getTaskTimeFilter = function () {
                return {
                    Start: scope.startDate,
                    End: scope.endDate,
                    T: scope.selectedTasksList.length > 0 ? scope.selectedTasksList : null,
                    W: scope.selectedWorkflowsList.length > 0 ? scope.selectedWorkflowsList : null
                };
            };

            //-------------------------------------------------------------------------
            // Sort Select
            //-------------------------------------------------------------------------
            function sortSelect() {
                var tmpAry = new Array();
                for (var i = 0; i < scope.workflowsList.length; i++) {
                    tmpAry[i] = new Array();
                    tmpAry[i][0] = scope.workflowsList[i];
                    tmpAry[i][1] = scope.workflowsList[i];
                }
                tmpAry.sort();
                scope.workflowsList = [];
                for (var j = 0; j < tmpAry.length; j++) {
                    scope.workflowsList.push(tmpAry[j][0]);
                }
                return;
            }

            //-------------------------------------------------------------------------
            // Ajax Call - Get report data
            //-------------------------------------------------------------------------
            var ajaxCallTaskTime = function () {

                var filter = getTaskTimeFilter();
                var json = filter;

                reportService.getTasks(json).then(function (data) {
                    // Success goes here
                    scope.tasksList = [];
                    scope.workflowsList = [];

                    // Initial google chart
                    scope.chartObject = {
                        options: {
                            title: 'Average task completion time',
                            vAxis: { title: "Time" },
                            legend: { position: 'none' },
                            colors: ['#c7cfc7', '#b2c8b2', '#d9e0de', '#cdded1'],
                            titleTextStyle: { fontSize: "18" }
                        }
                    };

                    scope.chartObject.data = {
                        'cols': [
                            { id: "t", label: "Task", type: "string" },
                            { id: "d", label: "Duration", type: "number" }
                        ],
                        'rows': []
                    };
                    scope.chartObject.type = "ColumnChart";

                    if (data.length !== 0) {
                        var wfcodes = "";
                        angular.forEach(data, function (rptRow) {
                            // push row
                            scope.chartObject.data.rows.push({
                                c: [
                                    { v: rptRow.task },
                                    { v: rptRow.duration }
                                ]
                            });

                            scope.tasksList.push(rptRow.task);

                            // make sure workflow codes are unique
                            if (!wfcodes.match(rptRow.workflow)) {
                                scope.workflowsList.push(rptRow.workflow);
                                wfcodes += '*-*' + rptRow.workflow;
                            }
                        });

                        sortSelect();

                        scope.selectedTasksList = [];
                        scope.selectedWorkflowsList = [];

                    }
                }, function (err) {
                    // Error goes here
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading tasks report');
                });

            }

            // Set start date default value is last month
            scope.startDate = moment().add(-1, 'months').format(globalConfig.momentDateFormat);
            // Set end date default value is current month
            scope.endDate = moment().format(globalConfig.momentDateFormat);
            // Datetime format
            scope.dateFormat = globalConfig.dateFormat;

            // Tasks
            scope.selectedTasksList = [];
            scope.tasksList = [];

            // Workflows
            scope.selectedWorkflowsList = [];
            scope.workflowsList = [];

            // Refresh
            scope.refresh = function () {
                rptTaskTime();
            };

            // Load reporter
            rptTaskTime();
        }
    }
}])

// Workflow
.directive('reportWorkflow', ['globalConfig', 'ReportService','myAlert', function (globalConfig, reportService,myAlert) {
    return {
        restrict: "A",
        templateUrl: 'scripts/flowTasksApp/views/report/index.workflow.html',
        scope: true,
        link: function (scope, elem) {

            //-------------------------------------------------------------------------
            // Report Workflow Time - callback
            // Callback that creates and populates a data table,
            // instantiates the pie chart, passes in the data and
            // draws it.
            //-------------------------------------------------------------------------
            var rptWorkflowTime = function () {

                // Ajax call to get report data and draw report
                ajaxCallWorkflowTime();

            }

            //-------------------------------------------------------------------------
            // Get Workflow Time Filter
            //-------------------------------------------------------------------------
            var getWorkflowTimeFilter = function () {
                return {
                    Start: scope.startDate,
                    End: scope.endDate,
                    W: scope.selectedWorkflowsList.length > 0 ? scope.selectedWorkflowsList : null
                };
            };

            //-------------------------------------------------------------------------
            // Ajax Call Workflow Time - Get report data
            //-------------------------------------------------------------------------
            var ajaxCallWorkflowTime = function (report, options) {

                var filter = getWorkflowTimeFilter();
                var json = filter;

                reportService.getWorkflows(json).then(function (data) {
                    // Success goes here

                    scope.workflowsList = [];

                    // Initial google chart
                    scope.chartObject = {
                        options: {
                            vAxis: { title: "Time" },
                            legend: { position: 'none' },
                            colors: ['#b2c8b2', '#d9e0de', '#cdded1', '#c7cfc7'],
                            titleTextStyle: { fontSize: "18" }
                        }
                    };

                    scope.chartObject.data = {
                        'cols': [
                            { id: "w", label: "Workflow", type: "string" },
                            { id: "d", label: "Duration", type: "number" }
                        ],
                        'rows': []
                    };
                    scope.chartObject.type = "ColumnChart";

                    if (data.length !== 0) {

                        angular.forEach(data, function (rptRow) {
                            // push row
                            scope.chartObject.data.rows.push({
                                c: [
                                    { v: rptRow.workflow },
                                    { v: rptRow.duration }
                                ]
                            });

                            scope.workflowsList.push(rptRow.workflow);
                        });

                        scope.selectedWorkflowsList = [];
                    }

                }, function (err) {
                    // Error goes here
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading workflows report');
                });

            }

            // Set start date default value is last month
            scope.startDate = moment().add(-1, 'months').format(globalConfig.momentDateFormat);
            // Set end date default value is current month
            scope.endDate = moment().format(globalConfig.momentDateFormat);
            // Datetime format
            scope.dateFormat = globalConfig.dateFormat;

            // Workflows
            scope.selectedWorkflowsList = [];
            scope.workflowsList = [];

            // Refresh
            scope.refresh = function () {
                rptWorkflowTime();
            };
            
            // Load reporter
            rptWorkflowTime();
        }
    }
}])