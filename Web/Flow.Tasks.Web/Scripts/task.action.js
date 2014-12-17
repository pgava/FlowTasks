$(document).ready(function () {
    var parentBody = window.parent.document.body;
    var btnValue = '',comment='';

    $("input[type='submit']").on("click", function () {
        if (jQuery(this).parents(".documentattached").length <= 0) {
            var keepTaskInList = jQuery(this).hasClass("keepTaskInList");
            btnValue = $(this).val();
            //comment = $(this).parents('body').html();

            if (!jQuery(this).hasClass("unvalidate") && typeof jQuery().valid !== 'undefined' && !jQuery("form#flowTasksTaskView").valid()) {
                return;
            }
            
            setTimeout(function () {
                //send button value to parent
                jQuery("#taskdetailiframe", parentBody).find("#iframeaction").val(btnValue);
                //confirm remove this task from list
                jQuery("#taskdetailiframe", parentBody).find("#keepTaskInList").val(keepTaskInList ? "1" : "0");
                //hide the iframe and calendar
                jQuery("#taskdetailiframe", parentBody).find("iframe").hide();
                //hide the 'back to list' button
                jQuery("#taskdetailcontrol", parentBody).hide();
                //show loading icon
                jQuery("#taskdetailiframe", parentBody).find("#taskview-iframe-loader").show();
                //scroll to loading
                if (jQuery('#task-detail', parentBody).offset().top < jQuery(parentBody).scrollTop()) {
                    jQuery(parentBody).scrollTop(jQuery('#task-detail', parentBody).offset().top - 80);
                }
            }, 100);
        }
    });


    $("form#flowTasksTaskView").submit(function () {
        
    });

    $(".urlHelp,.pathHelp").click(function (e) {
        e.preventDefault();
    });

    //$("input[type=text],input[type=file],textarea").focus(function () {
    //    setTimeout(function() {
    //        alert($(window).height());
    //    }, 800);
    //    //var currentScrollTop = $("#viewtaskdetail", parentBody).scrollTop();
    //    //alert(currentScrollTop);
    //    //$("#viewtaskdetail", parentBody).scrollTop();
    //});

    //var parentBody = window.parent.document.body;
    var previousHeight = $('body').height();
    $("#taskdetailiframe", parentBody).find("iframe").css("height", $('body').height() + 230 + "px");
    setInterval(function () {
        //console.log($('body').height() + " > " + previousHeight);
        if ($('body').height() > previousHeight) {
            previousHeight = $('body').height();
            $("#taskdetailiframe", parentBody).find("iframe").css("height", $('body').height() + 230 + "px");
        }
    }, 500);
    

    //open skwetch workflow
    jQuery(".downloadsketchworkflow a").click(function(e) {
        e.preventDefault();
        window.parent.location.href = window.parent.location.pathname + "#/sketch?workflow=" + jQuery("#SketchWorkflowCode").val();
    });
});

//$(window).load(function () {
//    var parentBody = window.parent.document.body;
//    $("#taskdetailiframe", parentBody).find("iframe").css("height", $(document).height() + 200 + "px");
//});