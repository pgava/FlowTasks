
$().ready(function () {
    $("#flowTasksTaskView").validate({
        errorLabelContainer: $("ul", $('div.error-container')),
        wrapper: 'li',
        rules: {
            TaskParameterCandidateName: {
                required: true
            },
            TaskParameterCandidateEmail: {
                required: true
            }
        },
        messages: {

            TaskParameterCandidateName: {
                required: "Please enter a candidate name"
            },
            TaskParameterCandidateEmail: {
                required: "Please enter a candidate email"
            }
        }
    });
});

