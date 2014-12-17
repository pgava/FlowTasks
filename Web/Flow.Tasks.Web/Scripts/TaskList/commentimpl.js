/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
*/

/*=============================================================================
Comment Implementation
==============================================================================*/
var commentImpl = function () {
    //-------------------------------------------------------------------------
    // Get Data
    //-------------------------------------------------------------------------
    var getData = function () {
        var search = {
            WorkflowOid: $("#WorkflowOid").val(),
            TaskOid: $("#TaskOid").val()
        };

        return JSON.stringify(search);
    };

    //-------------------------------------------------------------------------
    // Process Comment
    //-------------------------------------------------------------------------
    var processComment = function (comment) {
        var tooltip = "";
        if (comment.length > 60) {
            tooltip = '<a class="tooltipMessage" href="#" title="Message|';
            tooltip += comment;
            tooltip += '">';
            tooltip += comment.substring(0, 60);
            tooltip += '...</a>';
        }
        else {
            tooltip = comment;
        }

        return tooltip;
    };

    //-------------------------------------------------------------------------
    // Create Comment List
    //-------------------------------------------------------------------------
    var commentListForTask = function (comments) {
        var ctrlComments = $('#comments');
        ctrlComments.children().remove();

        var tableComments = '<table><tr><th>When</th><th>User</th><th>Message</th></tr>';

        var c;
        for (c = 0; c < comments.length; c += 1) {
            tableComments += '<tr><td>' + comments[c].When + '</td><td>' +
                comments[c].User + '</td><td>' + processComment(comments[c].Message) + '</td></tr>';
        }
        tableComments += '</table>';

        if (comments.length > 0) {
            ctrlComments.show();
            $('#no_comments').hide();
            ctrlComments.append(tableComments);

            if (comments.length > 10) {
                ctrlComments.css({'height': '150px'});
            }
        }
        else {
            $('#no_comments').show();
            ctrlComments.hide();
        }
    };

    //-------------------------------------------------------------------------
    // Create Comment List (Interface)
    //-------------------------------------------------------------------------
    var createCommentList = function () { };

    //-------------------------------------------------------------------------
    // Ajax Call
    //-------------------------------------------------------------------------
    var ajaxCall = function () {
        $.ajax({
            url: $("#MyURL").val() + '/GetComments',
            type: 'POST',
            dataType: 'json',
            data: getData(),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                createCommentList(data.Comments);

                // Cluetip for messages too long.
                $('a.tooltipMessage').each(function () {
                    $(this).cluetip({ splitTitle: '|', arrows: true, dropShadow: false, cluetipClass: 'jtip' });
                });

            }
        });
    };

    return {

        //-------------------------------------------------------------------------
        // Init Comments For Task
        //-------------------------------------------------------------------------
        initCommentsForTask: function () {
            // Copy user comment in hidden field for post back
            $(".taskComment").change(function () {
                $("#Comment_TaskComment").val($(this).val());
            });

            // comment this out for new ui
            // Default implementation
            //createCommentList = commentListForTask;

            //ajaxCall();
            //**************
        },

        //-------------------------------------------------------------------------
        // Init Comments Generic
        //   actionFormat: function that formats comments
        //-------------------------------------------------------------------------
        initCommentsGeneric: function (actionFormat) {

            createCommentList = actionFormat;

            ajaxCall();
        }
    };
};
