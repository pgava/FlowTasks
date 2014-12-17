
//-------------------------------------------------------------------------
// Report User Task Count - callback
// Callback that creates and populates a data table,
// instantiates the column chart, passes in the data and
// draws it.
//-------------------------------------------------------------------------
rptUserTaskCount = function () {

    // Create the data table.
    var report = new google.visualization.DataTable();
    report.addColumn('string', 'Task');

    // Set chart options
    var options = {
        'title': 'Tasks completed by user',
        'width': 800,
        'height': 400,
        titleTextStyle: { fontSize: "18" }
    };

    // Ajax call to get report data and draw report
    ajaxCallUserTaskCount(report, options);

}

//-------------------------------------------------------------------------
// Get User Task Count Filter
//-------------------------------------------------------------------------
var getUserTaskCountFilter = function () {
    return {
        Start: $("#Start_UserTaskCount").val(),
        End: $("#End_UserTaskCount").val(),
        Users: $("#Select_UserCount").val(),
        Tasks: $("#Select_TaskCount").val()
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
ajaxCallUserTaskCount = function (report, options) {

    var filter = getUserTaskCountFilter();
    var json = JSON.stringify(filter);

    $.ajax({
        url: $("#MyURL").val() + '/' + 'ReportUserTaskCount',
        type: 'POST',
        dataType: 'json',
        data: json,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            $('#Select_UserCount option').remove();
            $('#Select_TaskCount option').remove();

            if (data.length !== 0) {
                var users = [];
                var tasks = [];
                
                var usersStr = "";
                var tasksStr = "";

                // Get users/tasks
                $.each(data, function (idx, rptRow) {
                    if (!usersStr.match(rptRow.User)) {
                        users.push(rptRow.User);
                        usersStr += "*-*" + rptRow.User;
                    }
                    if (!tasksStr.match(rptRow.Task)) {
                        tasks.push(rptRow.Task);
                        tasksStr += "*-*" + rptRow.Task;
                    }
                });

                // Create columns for every user
                $.each(users, function (idx, u) {
                    report.addColumn('number', u);
                });

                // Set number of rows
                report.addRows(tasks.length);

                usersStr = "";
                tasksStr = "";

                $.each(data, function (idx, rptRow) {

                    report.setCell(jQuery.inArray(rptRow.Task, tasks), 0, rptRow.Task);
                    report.setCell(jQuery.inArray(rptRow.Task, tasks), jQuery.inArray(rptRow.User, users)+1, rptRow.Count);

                    // make sure users/tasks are unique
                    if (!usersStr.match(rptRow.User)) {
                        $("#Select_UserCount").append('<option value="' + rptRow.User + '">' + rptRow.User + '</option>');
                        usersStr += '*-*' + rptRow.User;
                    }
                    if (!tasksStr.match(rptRow.Task)) {
                        $("#Select_TaskCount").append('<option value="' + rptRow.Task + '">' + rptRow.Task + '</option>');
                        tasksStr += '*-*' + rptRow.Task;
                    }

                });

                $("#Select_UserCount").multiselect('refresh');
                $("#Select_TaskCount").multiselect('refresh');
            }

            // Instantiate and draw our chart, passing in some options.
            var chart = new google.visualization.ColumnChart(document.getElementById('chart_userTaskCount'));
            chart.draw(report, options);

        }
    });
}