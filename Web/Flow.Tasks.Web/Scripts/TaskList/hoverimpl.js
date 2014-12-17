/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
* Assume: A browser
*/

// TODO: Fixme!!!
var stopHoverFlag = false;

/*=============================================================================
Show Toolbox From Task
  tasklist: Task List Implementation
==============================================================================*/
var showToolboxFromTask = function (tasklist) {

    //-------------------------------------------------------------------------
    // Process Comments
    //-------------------------------------------------------------------------
    var processHasComments = function (el) {

        var comments = [],
            loop,
            c;

        for (loop = 0; loop < 10; loop += 1) {
            var keyWhen = 'Comments_' + loop + '__When';
            var keyUser = 'Comments_' + loop + '__User';
            var keyMsg = 'Comments_' + loop + '__Message';
            if (el.data(keyWhen)) {
                var cmt = {};
                cmt.When = el.data(keyWhen);
                cmt.User = el.data(keyUser);
                cmt.Message = el.data(keyMsg);
                comments.push(cmt);
            }
        }

        var commentsScroll = $('#scrollbar1');
        commentsScroll.children().remove();

        var tableComments = '<div class="scrollbar"><div class="track"><div class="thumb"><div class="end"></div></div></div></div><div class="viewport"><div class="overview">';
        for (c = 0; c < comments.length; c += 1) {
            tableComments += '<h5>' + comments[c].When + ' - ' + comments[c].User + '</h5>' + '<div>' +
                comments[c].Message + '</div>';
        }
        tableComments += '</div></div>';

        if (comments.length > 0) {
            commentsScroll.append(tableComments);
            $('#toolboxComments').show();
            $('#toolboxCommentsView').hide();
            // moved after
            //$('#scrollbar1').tinyscrollbar();

            return true;
        }

        return false;
    };

    return {
        func: function () {
            /*
            * this is "li"
            * $("#lblId").text($(this).find('a').attr('id'));
            */

            //console.log('showToolboxFromTask >> stopHoverFlag = ' + stopHoverFlag);

            if (stopHoverFlag) {
                return;
            }

            tasklist.removeHoverStyle();

            $("#lblTitle").text($(this).data('Title'));
            $("#lblDescription").text($(this).data('Description'));
            $("#lblExpires").text($(this).data('Expires'));
            $("#lblTaskOid").text($(this).data('TaskOid'));
            $("#lblIsAssigned").text($(this).data('IsAssigned'));

            if ($(this).data('IsAssigned') === "True") {
                $("#assignTask").hide();
                $("#givebackTask").show();
            }
            else {
                $("#assignTask").show();
                $("#givebackTask").hide();
            }

            var hasComments = processHasComments($(this));

            tasklist.showFromTask();

            $("#toolbox").show("highlight");

            if (hasComments) {
                $("#toolboxCommentsRow").show();
                $('#scrollbar1').tinyscrollbar();
            }
            else {
                $("#toolboxCommentsRow").hide();
            }
        }
    };
};

/*=============================================================================
Hide Toolbox From Task
  tasklist: Task List Implementation
==============================================================================*/
var hideToolboxFromTask = function (tasklist) {
    return {
        func: function () {

            tasklist.hideFromTask();

            if (tasklist.isHideFromTask()) {
                $("#toolbox").hide("highlight");
            }
        }
    };
};

/*=============================================================================
Show Toolbox From Itself
  tasklist: Task List Implementation
==============================================================================*/
var showToolboxFromItself = function (tasklist) {

    return {
        func: function () {

            if (stopHoverFlag) {
                return;
            }

            tasklist.setOverToolbox(true);

            tasklist.addHoverStyle();
        }
    };
};

/*=============================================================================
Hide Toolbox From Itself
  tasklist: Task List Implementation
==============================================================================*/
var hideToolboxFromItself = function (tasklist) {

    return {
        func: function () {

            tasklist.setOverToolbox(false);
            if (tasklist.isHideFromItself()) {
                $("#toolbox").hide("highlight");

                tasklist.removeHoverStyle();
            }
        }
    };
};

/*=============================================================================
Hover implementation
  tasklist: Task List Implementation
==============================================================================*/
var hoverImpl = function (tasklist) {

    var configTask = {
        over: showToolboxFromTask(tasklist).func,
        timeout: 600,
        out: hideToolboxFromTask(tasklist).func
    };

    var configToolbox = {
        over: showToolboxFromItself(tasklist).func,
        timeout: 500,
        out: hideToolboxFromItself(tasklist).func
    };

    return {

        //-------------------------------------------------------------------------
        // Init Hover
        //-------------------------------------------------------------------------
        initHover: function () {

            $("#toolbox").hide();

            $("#toolbox").hoverIntent(configToolbox);
            $("#button > ul").find('li').each(function () {
                var li = $(this);

                li.hoverIntent(configTask);
            });
        },

        //-------------------------------------------------------------------------
        // Stop Hover
        //-------------------------------------------------------------------------
        stopHover: function () {
            stopHoverFlag = true;
            
        },

        //-------------------------------------------------------------------------
        // Start Hover
        //-------------------------------------------------------------------------
        startHover: function () {
            stopHoverFlag = false;
        }
    };
};


