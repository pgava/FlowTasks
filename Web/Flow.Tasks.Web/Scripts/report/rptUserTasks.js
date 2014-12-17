
//-------------------------------------------------------------------------
// Report User Tasks - callback
// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
//-------------------------------------------------------------------------
rptUserTasks = function () {

    // Create the data table.
    var report = new google.visualization.DataTable();
    report.addColumn('string', 'User');
    report.addColumn('number', 'Tasks');

    // Set chart options
    var options = {
        'title': 'Total number of tasks completed by user',
        'width': 800,
        'height': 400,
        titleTextStyle: { fontSize: "18" }
    };

    // Ajax call to get report data and draw report
    ajaxCallUserTasks(report, options);

}

//-------------------------------------------------------------------------
// Get User Tasks Filter
//-------------------------------------------------------------------------
var getUserTasksFilter = function () {
    return {
        Start: $("#Start_UserTasks").val(),
        End: $("#End_UserTasks").val(),
        Users: $("#Select_UserTasks").val()
    };
};

//-------------------------------------------------------------------------
//Ajax Call User Tasks - Get report data
//-------------------------------------------------------------------------
ajaxCallUserTasks = function (report, options) {

    var filter = getUserTasksFilter();
    var json = JSON.stringify(filter);

    $.ajax({
        url: $("#MyURL").val() + '/' + 'ReportUserTasks',
        type: 'POST',
        dataType: 'json',
        data: json,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            $('#Select_UserTasks option').remove();

            if (data.length !== 0) {

                $.each(data, function (idx, rptRow) {
                    report.addRows([[rptRow.User, rptRow.TaskNo]]);

                    $("#Select_UserTasks").append('<option value="' + rptRow.User + '">' + rptRow.User + '</option>');
                });

                $("#Select_UserTasks").multiselect('refresh');
            }

            // Instantiate and draw our chart, passing in some options.
            var chart = new google.visualization.PieChart(document.getElementById('chart_userTasks'));
            chart.draw(report, options);

        }
    });
}