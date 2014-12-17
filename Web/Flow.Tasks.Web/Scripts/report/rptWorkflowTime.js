//-------------------------------------------------------------------------
// Report Workflow Time - callback
// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
//-------------------------------------------------------------------------
rptWorkflowTime = function () {

    // Create the data table.
    var report = new google.visualization.DataTable();
    report.addColumn('string', 'Workflow');
    report.addColumn('number', 'Duration');

    // Set chart options
    var options = {
        'title': 'Average workflow completion time',
        'width': 800,
        'height': 400,
        vAxis: { title: "Time" },
        legend: { position: 'none' },
        colors: ['#b2c8b2', '#d9e0de', '#cdded1', '#c7cfc7'],
        titleTextStyle: { fontSize: "18" }
    };

    // Ajax call to get report data and draw report
    ajaxCallWorkflowTime(report, options);

}

//-------------------------------------------------------------------------
// Get Workflow Time Filter
//-------------------------------------------------------------------------
var getWorkflowTimeFilter = function () {
    return {
        Start: $("#Start_WorkflowTime").val(),
        End: $("#End_WorkflowTime").val(),
        Workflows: $("#Select_WorkflowTime").val()
    };
};

//-------------------------------------------------------------------------
// Ajax Call Workflow Time - Get report data
//-------------------------------------------------------------------------
ajaxCallWorkflowTime = function (report, options) {

    var filter = getWorkflowTimeFilter();
    var json = JSON.stringify(filter);

    $.ajax({
        url: $("#MyURL").val() + '/' + 'ReportWorkflowTime',
        type: 'POST',
        dataType: 'json',
        data: json,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            $('#Select_WorkflowTime option').remove();

            if (data.length !== 0) {

                $.each(data, function (idx, rptRow) {
                    report.addRows([[rptRow.Workflow, rptRow.Duration]]);

                    $("#Select_WorkflowTime").append('<option value="' + rptRow.Workflow + '">' + rptRow.Workflow + '</option>');
                });

                $("#Select_WorkflowTime").multiselect('refresh');
            }

            // Instantiate and draw our chart, passing in some options.
            var chart = new google.visualization.ColumnChart(document.getElementById('chart_workflowTime'));
            chart.draw(report, options);
        }
    });
}