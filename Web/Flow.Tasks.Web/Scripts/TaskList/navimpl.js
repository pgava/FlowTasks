/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $ taskImpl hoverImpl filterImpl
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
* Assume: A browser
*/

/*=============================================================================
Navigation Implementation
  isHover: used to synchronize toolbox window
==============================================================================*/
var navImpl = function (isHover) {

    //-------------------------------------------------------------------------
    // Update Navigation
    //   navigation : Text to display
    //-------------------------------------------------------------------------
    var updateNavigation = function (navigation) {
        $('#navPosition').text(navigation);
    };

    //-------------------------------------------------------------------------
    // Ajax Call
    //   navigation : Text to display
    //-------------------------------------------------------------------------
    var ajaxCallNav = function (navigation) {
        var myTasks = taskImpl();
        var myFilter = filterImpl(true);

        var myHover = {};
        if (isHover) {
            myHover = hoverImpl(myTasks);
        }

        var filter = myFilter.getPageFilter();

        filter.Navigation = navigation;

        var json = JSON.stringify(filter);

        $.ajax({
            url: $("#MyURL").val() + '/NavigationTask',
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                myTasks.createTaskList(data.Tasks);
                myTasks.initTaskList();
                if (isHover) {
                    myHover.initHover();
                }

                if (data.Tasks.length > 0) {
                    var f = data.Tasks[0].Filter;
                    updateNavigation(f.NavPosition);
                }
            }
        });
    };

    return {
        //-------------------------------------------------------------------------
        // Init Nav
        //-------------------------------------------------------------------------
        initNav: function () {

            $("#navFirst").click(function () {
                ajaxCallNav("First");
            });

            $("#navPrev").click(function () {
                ajaxCallNav("Prev");
            });

            $("#navNext").click(function () {
                ajaxCallNav("Next");
            });

            $("#navLast").click(function () {
                ajaxCallNav("Last");
            });
        }
    };
};