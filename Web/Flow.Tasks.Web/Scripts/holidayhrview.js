
$().ready(function () {

    $('a.urlHelp').cluetip({ splitTitle: '|', cluetipClass: 'jtip' });
    $('a.pathHelp').cluetip({ splitTitle: '|', cluetipClass: 'jtip' });

    $("#flowTasksTaskView").validate({
        errorLabelContainer: $("ul", $('div.error-container')),
        wrapper: 'li',
        rules: {
            workflowFile: {
                required: true
            },
            TaskParameterSketchWorkflowCode: {
                required: true
            },
            TaskParameterSketchWorkflowPath: {
                required: true
            },
            TaskParameterSketchWorkflowUrl: {
                required: true
            }
        },
        messages: {
            workflowFile: {
                required: "Please enter the workflow to upload"
            },
            TaskParameterSketchWorkflowCode: {
                required: "Please enter a workflow code"
            },
            TaskParameterSketchWorkflowPath: {
                required: "Please enter the path, in the server, where to upload the workflow"
            },
            TaskParameterSketchWorkflowUrl: {
                required: "Please enter the url for the new workflow service"
            }
        }
    });
});

