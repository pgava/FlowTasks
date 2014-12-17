/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $ taskImpl
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
* Assume: A browser
*/

/*=============================================================================
Toolbox features
==============================================================================*/
var toolboxFeatures = function () {
    //-------------------------------------------------------------------------
    // Get Task
    //-------------------------------------------------------------------------
    var getTask = function () {
        return $("#lblTaskOid").text();
    };

    //-------------------------------------------------------------------------
    // Get Task Obj
    //-------------------------------------------------------------------------
    var getTaskObj = function () {
        return (getTask() === "") ? null : { taskOid: getTask() };
    };

    return {

        //-------------------------------------------------------------------------
        // Init Toolbox Features
        //-------------------------------------------------------------------------
        initToolboxFeatures: function () {
            $("#toolboxAssign").click(function () {
                var task = getTaskObj();

                var json = JSON.stringify(task);

                $.ajax({
                    url: $("#MyURL").val() + '/AssignTask',
                    type: 'POST',
                    dataType: 'json',
                    data: json,
                    contentType: 'application/json; charset=utf-8',
                    success: function () {
                        var taskList = taskImpl();
                        taskList.setAssignFlag("True");
                        taskList.addSelectedStyle();

                        $("#assignTask").hide();
                        $("#givebackTask").show();

                    }
                });
            });

            $("#toolboxGiveBack").click(function () {
                var task = getTaskObj();

                var json = JSON.stringify(task);

                $.ajax({
                    url: $("#MyURL").val() + '/GiveBackTask',
                    type: 'POST',
                    dataType: 'json',
                    data: json,
                    contentType: 'application/json; charset=utf-8',
                    success: function () {
                        var taskList = taskImpl();
                        taskList.setAssignFlag("False");
                        taskList.removeSelectedStyle();

                        $("#assignTask").show();
                        $("#givebackTask").hide();

                    }
                });
            });

            var ctrlComments = $('#toolboxComments');
            ctrlComments.hide();

        }

    };
};
