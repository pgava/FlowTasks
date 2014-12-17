//-------------------------------------------------------------------------
// Report Task Time - callback
// Callback that creates and populates a data table,
// instantiates the pie chart, passes in the data and
// draws it.
//-------------------------------------------------------------------------
rptTaskTime = function () {

    // Create the data table.
    var report = new google.visualization.DataTable();
    report.addColumn('string', 'Task');
    report.addColumn('number', 'Duration');

    // Set chart options
    var options = {
        'title': 'Average task completion time',
        'width': 800,
        'height': 400,
        vAxis: { title: "Time" },
        legend: { position: 'none' },
        colors: ['#c7cfc7', '#b2c8b2', '#d9e0de', '#cdded1'],
        titleTextStyle: { fontSize: "18" }
    };

    // Ajax call to get report data and draw report
    ajaxCallTaskTime(report, options);

}

//-------------------------------------------------------------------------
// Get Filter
//-------------------------------------------------------------------------
var getTaskTimeFilter = function () {
    return {
        Start: $("#Start_TaskTime").val(),
        End: $("#End_TaskTime").val(),
        Tasks: $("#Select_TaskTime").val(),
        Workflows: $("#Select_TaskWfTime").val()
    };
};

//-------------------------------------------------------------------------
// Sort Select
//-------------------------------------------------------------------------
function sortSelect(selElem) {
    var tmpAry = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpAry[i] = new Array();
        tmpAry[i][0] = selElem.options[i].text;
        tmpAry[i][1] = selElem.options[i].value;
    }
    tmpAry.sort();
    while (selElem.options.length > 0) {
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpAry.length; i++) {
        var op = new Option(tmpAry[i][0], tmpAry[i][1]);
        selElem.options[i] = op;
    }
    return;
}

//-------------------------------------------------------------------------
// Ajax Call - Get report data
//-------------------------------------------------------------------------
ajaxCallTaskTime = function (report, options) {

    var filter = getTaskTimeFilter();
    var json = JSON.stringify(filter);

    $.ajax({
        url: $("#MyURL").val() + '/' + 'ReportTaskTime',
        type: 'POST',
        dataType: 'json',
        data: json,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            $('#Select_TaskTime option').remove();
            $('#Select_TaskWfTime option').remove();

            if (data.length !== 0) {
                var wfcodes = "";
                $.each(data, function (idx, rptRow) {
                    report.addRows([[rptRow.Task, rptRow.Duration]]);

                    $("#Select_TaskTime").append('<option value="' + rptRow.Task + '">' + rptRow.Task + '</option>');

                    // make sure workflow codes are unique
                    if (!wfcodes.match(rptRow.Workflow)) {
                        $("#Select_TaskWfTime").append('<option value="' + rptRow.Workflow + '">' + rptRow.Workflow + '</option>');
                        wfcodes += '*-*' + rptRow.Workflow;
                    }
                });

                sortSelect(document.getElementById('Select_TaskWfTime'));

                $("#Select_TaskTime").multiselect('refresh');
                $("#Select_TaskWfTime").multiselect('refresh');

            }

            // Instantiate and draw our chart, passing in some options.
            var chart = new google.visualization.ColumnChart(document.getElementById('chart_taskTime'));
            chart.draw(report, options);
        }
    });
}