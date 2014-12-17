/*
* Checked with JsLint: http://www.jslint.com/ (21/02/2013)
* Predefined: jQuery $
* Tolerate: Missing 'use strict' pragma
*           many var statements per function
*           messy white space
* Assume: A browser
*/

/*=============================================================================
Task Element Implementation
==============================================================================*/
var taskElementImpl = function () {
    return {

        //-------------------------------------------------------------------------
        // Create Task Element. Create an html list item with an embedded form for 
        // the specified task.
        //   task: the task element
        //   ul: the list to add the task
        //-------------------------------------------------------------------------
        createTaskElement: function (task, ul) {

            var form = '<form method="post" action="'+ $("#MyURL").val() +'/TaskSelect">';
            var span = '<span class="executeTask" title="Quich run...">&nbsp;</span>';
            var li = '<li><a href="#" id="TaskOid" class="taskLink">' + task.Title + '**SPAN**</a>';

            $.each(task, function (key, value) {
                if (key === "Filter") {
                    $.each(this, function (subKey, subValue) {
                        var myval = subValue;
                        myval = myval === true ? "True" : myval;
                        myval = myval === false ? "False" : myval;

                        var hidden = '<input name="Filter.' + subKey + '" id="Filter_' + subKey + '" type="hidden" value="' + myval + '">';

                        li += hidden;
                    });
                }
                else if (key === "Comment") {
                    $.each(this, function (subKey, subValue) {
                        if (subKey === "Comments") {
                            var counter = 0;
                            $.each(subValue, function (index, comment) {
                                var myval = comment;
                                myval = myval === true ? "True" : myval;
                                myval = myval === false ? "False" : myval;

                                var paramId = 'Comments_' + index + '__';

                                var hidden = '<input id="' + paramId + 'When" name="' + paramId + 'When" type="hidden" value="' + myval.When + '">';
                                li += hidden;

                                hidden = '<input id="' + paramId + 'User" name="' + paramId + 'User" type="hidden" value="' + myval.User + '">';
                                li += hidden;

                                hidden = '<input id="' + paramId + 'Message" name="' + paramId + 'Message" type="hidden" value="' + myval.Message + '">';
                                li += hidden;

                                counter += 1;
                            });
                        }
                        else {
                            var myval = subValue;
                            myval = myval === true ? "True" : myval;
                            myval = myval === false ? "False" : myval;

                            var hidden = '<input name="Comment.' + subKey + '" id="Comment_' + subKey + '" type="hidden" value="' + myval + '">';

                            li += hidden;
                        }
                    });
                }
                else if (key === "Parameters") {
                    var counter = 0;
                    $.each(value, function (index, subValue) {
                        var myval = subValue;
                        myval = myval === true ? "True" : myval;
                        myval = myval === false ? "False" : myval;

                        var paramId = 'Parameters[' + index + ']';
                        var hidden = '<input id="' + paramId + '.Name" name="' + paramId + '.Name" type="hidden" value="' + myval.Name + '">';
                        li += hidden;

                        hidden = '<input id="' + paramId + '.Value" name="' + paramId + '.Value" type="hidden" value="' + myval.Value + '">';
                        li += hidden;

                        counter += 1;
                    });
                }
                else if (key === "Documents") {
                    var counter = 0;
                    $.each(value, function (index, subValue) {
                        var myval = subValue;
                        myval = myval === true ? "True" : myval;
                        myval = myval === false ? "False" : myval;

                        var paramId = 'Documents[' + index + ']';
                        var hidden = '<input id="' + paramId + '.DocumentName" name="' + paramId + '.DocumentName" type="hidden" value="' + myval.DocumentName + '">';
                        li += hidden;

                        hidden = '<input id="' + paramId + '.DocumentOid" name="' + paramId + '.DocumentOid" type="hidden" value="' + myval.DocumentOid + '">';
                        li += hidden;

                        counter += 1;
                    });
                }
                else {
                    var myval = value;
                    myval = myval === true ? "True" : myval;
                    myval = myval === false ? "False" : myval;

                    var hidden = '<input name="' + key + '" id="' + key + '" type="hidden" value="' + myval + '">';

                    if (key === "DefaultResult" && myval) {
                        li = li.replace("**SPAN**", span);
                    }

                    li += hidden;
                }
            });

            li += '</li>';

            li = li.replace("**SPAN**", "");
            form += li;
            form += '</form>';

            ul.append(form);

        },

        //-------------------------------------------------------------------------
        // Init Task Element
        //   li: the list item with the task
        //-------------------------------------------------------------------------
        initTaskElement: function (li) {
            li.find(":hidden").each(function () {
                li.data($(this).attr('id'), $(this).val());

                // if task assigned then highlight it 
                if ($(this).attr('id') === "IsAssigned" && $(this).val() === "True") {

                    li.find('a').each(function () {
                        $(this).addClass("selected");
                    });
                }
            });
        },

        //-------------------------------------------------------------------------
        // Remove Hover Style
        //-------------------------------------------------------------------------
        removeHoverStyle: function () {
            $("#button > ul").find(":hidden").each(function () {
                if ($(this).attr('id') === "TaskOid") {
                    if ($(this).val() === $("#lblTaskOid").text()) {
                        $(this).parent().find('a').each(function () {
                            $(this).removeClass("hover_selected_style");
                            $(this).removeClass("hover_style");
                        });

                    }
                }
            });
        },

        //-------------------------------------------------------------------------
        // Remove Selected Style
        //-------------------------------------------------------------------------
        removeSelectedStyle: function () {
            $("#button > ul").find(":hidden").each(function () {
                if ($(this).attr('id') === "TaskOid") {
                    if ($(this).val() === $("#lblTaskOid").text()) {
                        $(this).parent().find('a').each(function () {
                            $(this).removeClass("selected");
                            $(this).removeClass("hover_selected_style");
                            $(this).addClass("hover_style");
                        });

                    }
                }
            });
        },

        //-------------------------------------------------------------------------
        // Add Hover Style
        //-------------------------------------------------------------------------
        addHoverStyle: function () {
            $("#button > ul").find(":hidden").each(function () {
                if ($(this).attr('id') === "TaskOid") {
                    if ($(this).val() === $("#lblTaskOid").text()) {
                        var hoverClass = "";
                        if ($("#lblIsAssigned").text() === "True") {
                            hoverClass = "hover_selected_style";
                        }
                        else {
                            hoverClass = "hover_style";
                        }

                        $(this).parent().find('a').each(function () {
                            $(this).addClass(hoverClass);
                        });
                    }
                }
            });
        },

        //-------------------------------------------------------------------------
        // Add Selected Style
        //-------------------------------------------------------------------------
        addSelectedStyle: function () {
            $("#button > ul").find(":hidden").each(function () {
                if ($(this).attr('id') === "TaskOid") {
                    if ($(this).val() === $("#lblTaskOid").text()) {

                        $(this).parent().find('a').each(function () {
                            $(this).addClass("selected");
                            $(this).addClass("hover_selected_style");
                            $(this).removeClass("hover_style");
                        });
                    }
                }
            });
        },

        //-------------------------------------------------------------------------
        // Set Assign Flag
        //   isAssign: isAssign flag
        //-------------------------------------------------------------------------
        setAssignFlag: function (isAssign) {
            $("#button > ul").find(":hidden").each(function () {
                if ($(this).attr('id') === "TaskOid") {
                    if ($(this).val() === $("#lblTaskOid").text()) {
                        $(this).parent().data("IsAssigned", isAssign);
                        $(this).parent().find(":hidden").each(function () {
                            if ($(this).attr('id') === "IsAssigned") {
                                $(this).val(isAssign);
                            }
                        });
                    }
                }
            });
        }
    };
};

/*=============================================================================
Task Implementation
==============================================================================*/
var taskImpl = function () {

    // over Toolbox
    var overToolbox = false;

    // counter Li
    var counterLi = 0;

    //-------------------------------------------------------------------------
    // Get Task
    //   el : control has been clicked
    //-------------------------------------------------------------------------
    var getTask = function (el) {
        var taskOid,
            workflowOid,
            correlationId,
            taskCode,
            defaultResult;

        var listHidden = el.closest("li").find(":hidden");

        listHidden.each(function () {
            if ($(this).attr("id") === "TaskOid") {
                taskOid = $(this).val();
            }
            else if ($(this).attr("id") === "WorkflowOid") {
                workflowOid = $(this).val();
            }
            else if ($(this).attr("id") === "TaskCorrelationId") {
                correlationId = $(this).val();
            }
            else if ($(this).attr("id") === "TaskCode") {
                taskCode = $(this).val();
            }
            else if ($(this).attr("id") === "DefaultResult") {
                defaultResult = $(this).val();
            }
        });

        return {
            TaskOid: taskOid,
            WorkflowOid: workflowOid,
            CorrelationId: correlationId,
            TaskCode: taskCode,
            DefaultResult: defaultResult
        };
    };

    //-------------------------------------------------------------------------
    // Quick Run Task
    //   el : control has been clicked
    //-------------------------------------------------------------------------
    var quickRunTask = function (el) {

        var task = getTask(el);
        var json = JSON.stringify(task);

        $.ajax({
            url: $("#MyURL").val() + '/QuickRunTask',
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                // Refresh task list
                myFilter = filterImpl();
                myFilter.refreshTaskList();
            }
        });
    };

    return {
        //-------------------------------------------------------------------------
        // Show From Task
        //-------------------------------------------------------------------------
        showFromTask: function () {
            counterLi += 1;
        },

        //-------------------------------------------------------------------------
        // Hide From Task
        //-------------------------------------------------------------------------
        hideFromTask: function () {
            counterLi -= 1;
        },

        //-------------------------------------------------------------------------
        // Is Hide From Task
        //-------------------------------------------------------------------------
        isHideFromTask: function () {
            return !overToolbox && counterLi === 0;
        },

        //-------------------------------------------------------------------------
        // Is Hide From Itself
        //-------------------------------------------------------------------------
        isHideFromItself: function () {
            return counterLi === 0;
        },

        //-------------------------------------------------------------------------
        // Set Over Toolbox
        //-------------------------------------------------------------------------
        setOverToolbox: function (val) {
            overToolbox = val;
        },

        //-------------------------------------------------------------------------
        // Get Over Toolbox
        //-------------------------------------------------------------------------
        getOverToolbox: function () {
            return overToolbox;
        },

        //-------------------------------------------------------------------------
        // Get Counter Li
        //-------------------------------------------------------------------------
        getCounterLi: function () {
            return counterLi;
        },

        //-------------------------------------------------------------------------
        // Create Task List
        //   tasks: list of tasks
        //-------------------------------------------------------------------------
        createTaskList: function (tasks) {
            var el = taskElementImpl();

            var ul = $('#button > ul');
            ul.children().remove();

            var taskCounter;
            for (taskCounter = 0; taskCounter < tasks.length; taskCounter += 1) {
                el.createTaskElement(tasks[taskCounter], ul);
            }
        },

        //-------------------------------------------------------------------------
        // Init Task List
        //-------------------------------------------------------------------------
        initTaskList: function () {
            $("a.taskLink").each(function () {

                var li = $(this).parent();
                var el = taskElementImpl();

                el.initTaskElement(li);

                // Submit form
                $(this).bind('click', function (ev) {
                    ev.preventDefault();
                    $(this).closest('form').submit();
                });

                // Add/Remove class for quick run 
                $(this).hover(
                    function () {
                        $(this).children("span").addClass("quickRun");
                    },
                    function () {
                        $(this).children("span").removeClass("quickRun");
                    }
                );
            });

            // Quick run
            $("span.executeTask").each(function () {
                var span = $(this);

                $(this).bind('click', function (ev) {
                    // Stop event to propagate to parent (<a>)
                    ev.stopPropagation();
                    
                    quickRunTask(span);
                });
            });
        },

        //-------------------------------------------------------------------------
        // Remove Hover Style
        //-------------------------------------------------------------------------
        removeHoverStyle: function () {
            var el = taskElementImpl();

            el.removeHoverStyle();
        },

        //-------------------------------------------------------------------------
        // Remove Selected Style
        //-------------------------------------------------------------------------
        removeSelectedStyle: function () {
            var el = taskElementImpl();

            el.removeSelectedStyle();
        },

        //-------------------------------------------------------------------------
        // Add Hover Style
        //-------------------------------------------------------------------------
        addHoverStyle: function () {
            var el = taskElementImpl();

            el.addHoverStyle();
        },

        //-------------------------------------------------------------------------
        // Add Selected Style
        //-------------------------------------------------------------------------
        addSelectedStyle: function () {
            var el = taskElementImpl();

            el.addSelectedStyle();
        },

        //-------------------------------------------------------------------------
        // Set Assign Flag
        //-------------------------------------------------------------------------
        setAssignFlag: function (isAssign) {
            var el = taskElementImpl();

            el.setAssignFlag(isAssign);
        }
    };
};
