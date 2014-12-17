//-------------------------------------------------------------------------
// Report JS
//-------------------------------------------------------------------------
$(document).ready(function () {

    $(".datePicker").datepicker();
    $(".datePicker").datepicker("option", "dateFormat", "dd/mm/yy");

    $("select").multiselect({
        selectedText: "# of # checked"
    });

    $("#Select_UserTasks").multiselect({
        noneSelectedText: "Select users below"
    });

    $("#Select_TaskTime").multiselect({
        noneSelectedText: "Select tasks below"
    });

    $("#Select_TaskWfTime").multiselect({
        noneSelectedText: "Select workflows below"
    });

    $("#Select_WorkflowTime").multiselect({
        noneSelectedText: "Select workflows below"
    });

    $("#Select_UserCount").multiselect({
        noneSelectedText: "Select users below"
    });

    $("#Select_TaskCount").multiselect({
        noneSelectedText: "Select tasks below"
    });

    $("#refresh_UserTasks").click(rptUserTasks);
    $("#refresh_TaskTime").click(rptTaskTime);
    $("#refresh_WorkflowTime").click(rptWorkflowTime);
    $("#refresh_UserTaskCount").click(rptUserTaskCount);

    var myNavbarImpl = navbarImpl();
    myNavbarImpl.setReportActive();
});

