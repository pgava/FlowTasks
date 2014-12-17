/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $ taskImpl hoverImpl
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
* Assume: A browser
*/

/*=============================================================================
Filter Implementation
==============================================================================*/
var filterImpl = function (hoverToolbox) {

    // Hover toolbox
    var hoverToolboxLocal = hoverToolbox;

    //-------------------------------------------------------------------------
    // Update Navigation
    //   navigation : Text to display
    //-------------------------------------------------------------------------
    var updateNavigation = function (navigation) {
        $('#navPosition').text(navigation);
    };

    //-------------------------------------------------------------------------
    // Get Filter
    //   displayMethod : Display Method
    //-------------------------------------------------------------------------
    var getFilter = function (displayMethod) {
        return {
            Filter: $("#Filter").val(),
            DisplayMethod: displayMethod,
            Domain: $("#Domain").val(),
            OrderMethod: $("#OrderMethod").val(),
            CurrentPage: $("#Filter_CurrentPage").val(),
            FilteredPages: $("#Filter_FilteredPages").val(),
            TotalPages: $("#Filter_TotalPages").val(),
            MaxTasks: $("#MaxTasks").val()
        };
    };

    //-------------------------------------------------------------------------
    // Enable Hover
    //-------------------------------------------------------------------------
    var enableHover = function () {
        hoverToolboxLocal.initHover();
        hoverToolboxLocal.startHover();
    };

    //-------------------------------------------------------------------------
    // Ajax Call
    //   displayMethod : Display Method
    //-------------------------------------------------------------------------
    var ajaxCall = function (fromStart, displayMethod) {
        var myTasks = taskImpl();

        hoverToolboxLocal = hoverImpl(myTasks);
        hoverToolboxLocal.stopHover();

        var filter = getFilter(displayMethod);
        if (fromStart) {
            filter.CurrentPage = "1";
        }

        var json = JSON.stringify(filter);

        $.ajax({
            url: $("#MyURL").val() + '/FilterTask',
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                myTasks.createTaskList(data.Tasks);
                myTasks.initTaskList();

                if (data.Tasks.length > 0) {
                    if (data.Tasks.length > 0) {
                        var f = data.Tasks[0].Filter;
                        updateNavigation(f.NavPosition);
                    }
                    $("#paging").show();
                }
                else {
                    $("#paging").hide();
                }

                setTimeout(enableHover, 1500);
            }
        });
    };

    return {
        //-------------------------------------------------------------------------
        // Init Filter
        //-------------------------------------------------------------------------
        initFilter: function () {

            $("#Filter").blur(function () {
                ajaxCall(true);
            });

            $("#OrderMethod").change(function () {
                ajaxCall(true);
            });

            $("#Domain").change(function () {
                ajaxCall(true);
            });

            $("#MaxTasks").change(function () {
                ajaxCall(true);
            });

            $("#filterAll").click(function () {
                ajaxCall(true, "All");
            });

            $("#filterDueToday").click(function () {
                ajaxCall(true, "DueToday");
            });

            $("#filterDueTomorrow").click(function () {
                ajaxCall(true, "DueTomorrow");
            });

            $("#filterOverdue").click(function () {
                ajaxCall(true, "Overdue");
            });
        },

        //-------------------------------------------------------------------------
        // Get Page Filter
        //-------------------------------------------------------------------------
        getPageFilter: function () {
            return getFilter();
        },

        //-------------------------------------------------------------------------
        // Refresh Task List
        //-------------------------------------------------------------------------
        refreshTaskList: function () {
            //TODO: remember display method!
            return ajaxCall(false);
    }

    };
};