flowTasksApp
    // Task box
    .directive("taskBox", [
        '$document', 'WorkContextSrv', 'TaskSrv', '$timeout', '$window', '$rootScope', 'ApplicationFactory', 'myAlert', 'TaskFactory',
        function ($document, workContextSrv, taskSrv, $timeout, $window, $rootScope, applicationFactory, myAlert, taskFactory) {
            return {
                scope: {
                    task: '=',
                    taskList: '=',
                    index: '@',
                    openTaskDetails: '&'
                },
                replace: true,
                restrict: 'A',
                templateUrl: 'scripts/flowTasksApp/views/task/box.html',
                link: function (scope, element, attrs) {

                    // fn check assign status
                    var checkAssigned = function (t) {
                        return typeof t.task != 'undefined' && applicationFactory.userData.userName == t.task.acceptedBy;
                    }

                    // fn show comment box
                    var showTaskComments = function (isShow) {
                        if (isShow) {
                            angular.element(element).find(".task-comments").show();

                            //add 'slidein' class to comments box
                            angular.element(element).find(".task-comments").addClass("slidein");
                        } else {
                            //hide task comments box
                            angular.element(element).find(".task-comments").hide();

                            //remove 'slidein' class from comments box
                            angular.element(element).find(".task-comments").removeClass("slidein");
                        }
                    }

                    // fn view task details
                    var viewTaskDetail = function (t) {
                        //on submitting state
                        scope.taskComment.onSubmitting = true;

                        scope.taskComment.hasErrors = false;

                        //hide the message
                        scope.taskComment.showMessage = false;

                        //scroll to top when view detail
                        t.scrollTop = angular.element($window).scrollTop();

                        //post assign request
                        taskSrv.taskOperation(t.task.taskOid, 'assign', applicationFactory.userData.userName, '', '')
                            .then(function (dt) {
                                if (dt === '') {
                                    //open task detail
                                    // set index
                                    t.index = scope.index;

                                    // open task details
                                    scope.openTaskDetails({ task: t });

                                    //set 'Assigned' state to current task
                                    scope.taskDetail.task.isAssigned = true;
                                    scope.task.task.isAssigned = true;
                                } else {
                                    //alert erro message
                                    myAlert("E", "Error", dt.message);
                                }
                                //off submitting state
                                scope.taskComment.onSubmitting = false;
                            }, function () {
                                //off submitting state
                                scope.taskComment.onSubmitting = false;
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading assigning task');
                            });
                    }

                    // Watch reload tasks event
                    $rootScope.$on("TASKS:RELOAD", function () {
                        scope.taskComment.onSubmitting = false;
                        scope.taskComment.loadingComments = false;
                        scope.taskComment.onViewDetail = false;
                        scope.taskComment.showMessage = false;
                        scope.taskComment.hasErrors = false;
                        scope.taskComment.errorMessage = '';
                        scope.taskDetail = {};
                    });
                    
                    scope.taskComment = {
                        onSubmitting: false,                                    // The value indicating whether task comment is submiting
                        loadingComments: false,                                 // The value indicating whether on loading comment
                        onViewDetail: false,                                    // The value indicating whether on viewing details
                        showMessage: false,                                     // The value indicating whether show message
                        hasErrors: false,                                       // The value indicating whether has errors
                        errorMessage: ''                                        // Error message
                    };

                    scope.taskDetail = {};                                      // Task details view model

                    // fn expand function
                    scope.expandTaskBox = function ($event, tp) {
                        // hide the message
                        scope.taskComment.showMessage = false;

                        // stop expanding when user click to loading overlay, attach files and complete button
                        if (angular.element($event.target).hasClass('task-box-loading-comments') ||
                            angular.element($event.target).hasClass('btnsavetaskfile') ||
                            angular.element($event.target).hasClass('lbassigned')) {
                            return;
                        }

                        // if task comments box has expanded
                        if (angular.element(element).find(".task-comments").hasClass("slidein")) {
                            // hide task comment
                            showTaskComments(false);
                        } else {
                            // on loadingComment state
                            scope.taskComment.loadingComments = true;

                            // post get task by oid request
                            taskSrv.getTaskByToid(tp.task.taskOid).then(function (dt) {
                                //bind taskdetail to model
                                scope.taskDetail = dt;

                                //bind current index of task in list
                                scope.taskDetail.index = attrs.tindex;

                                //off loadingComment state
                                scope.taskComment.loadingComments = false;

                                //show comments box
                                showTaskComments(true);
                            }, function (err) {
                                //off loadingComment state
                                scope.taskComment.loadingComments = false;
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading task comments');
                            });
                        }
                    };

                    // fn open task detail popup
                    scope.viewTaskDetail = function (t) {
                        viewTaskDetail(t);
                    };

                    // fn quick view task details
                    scope.quickViewTaskDetail = function (t) {
                        //on loadingComment state
                        scope.taskComment.loadingComments = true;

                        // do request get task by toid
                        taskSrv.getTaskByToid(t.task.taskOid).then(function (dt) {
                            //bind taskdetail to model
                            scope.taskDetail = dt;

                            //bind current index of task in list
                            scope.taskDetail.index = attrs.tindex;

                            //on loadingComment state
                            scope.taskComment.loadingComments = false;

                            // view task details
                            viewTaskDetail(t);
                        }, function (err) {
                            //on loadingComment state
                            scope.taskComment.loadingComments = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while loading task details');
                        });
                    }

                    // fn check assign status
                    scope.isTaskAssignedToMe = function (t) {
                        return checkAssigned(t);
                    };

                    // fn give back task
                    scope.giveBackTask = function (t) {

                        scope.taskComment.hasErrors = false;

                        // hide the message
                        scope.taskComment.showMessage = false;

                        // on submitting state
                        scope.taskComment.onSubmitting = true;

                        // operation
                        var operation = 'giveback';

                        //post giveback request
                        taskSrv.taskOperation(t.task.taskOid, operation, applicationFactory.userData.userName, '', '')
                            .then(function (dt) {
                                //show the message
                                scope.taskComment.showMessage = true;
                                if (dt === '') {
                                    //set 'Assigned' state to current task
                                    scope.taskDetail.task.isAssigned = false;
                                    scope.task.task.isAssigned = false;

                                    //bind the success message
                                    scope.taskComment.errorMessage = "Giving back task successful";
                                } else {
                                    //show the error message
                                    scope.taskComment.hasErrors = true;

                                    //bind the error message
                                    scope.taskComment.errorMessage = dt.message;
                                }

                                //update user stat
                                $rootScope.$broadcast("loadUserStat");

                                //off submitting state
                                scope.taskComment.onSubmitting = false;

                                //Re-update task counter
                                $rootScope.$broadcast("getTaskCount", false);
                            }, function (dt, st) {
                                //show the error message
                                if (angular.isDefined(dt.message))
                                    myAlert.pop("e", "Error", dt.message);
                                else
                                    myAlert.pop("e", "Error", dt);
                                //off submitting state
                                scope.taskComment.onSubmitting = false;
                            });
                    };

                    // fn complete task
                    scope.completeTask = function (t) {
                        //on loadingComment state
                        scope.taskComment.loadingComments = true;

                        //operation
                        var operation = 'approve';

                        //post approve request
                        taskSrv.taskOperation(t.task.taskOid, operation, applicationFactory.userData.userName, t.task.defaultResult, 'quick run task')
                            .then(function (dt, st) {
                                if (dt === '') {
                                    //remove task from list
                                    if (attrs.tindex > -1) {
                                        scope.taskList.results.splice(attrs.tindex, 1);
                                    }

                                    //show the success message
                                    myAlert.pop("s", "Success", "Completing task successful");
                                } else {
                                    //show the error message
                                    myAlert.pop("e", "Error", dt.message);
                                }

                                //update user stat
                                taskFactory.loadUserStat();

                                //off loadingComment state
                                scope.taskComment.loadingComments = false;

                                //Re-update task counter
                                taskFactory.setTaskCounter(taskFactory.counter - 1, false);
                            }, function (dt) {
                                //show the error message
                                if (angular.isDefined(dt.message))
                                    myAlert.pop("e", "Error", dt.message);
                                else
                                    myAlert.pop("e", "Error", dt);

                                //off loadingComment state
                                scope.taskComment.loadingComments = false;
                            });
                    };
                }
            };
        }
    ])

    // Hide task comment
    .directive("hideTaskComment", ['$document', function ($document) {
        return {
            restrict: "A",
            link: function (scope, elem) {
                //collapse if click outside task box
                $document.bind('click', function (e) {
                    //stop collapse comment boxs when user click to comments box, task detail is openning,
                    if (!angular.element(e.target).parents(".task-message").length > 0 &&
                        !angular.element(e.target).parents(".task-comments").length > 0 &&
                        !angular.element(e.target).hasClass('task-box-loading-comments') &&
                        !angular.element(e.target).parent("#taskdetailcontrol").length > 0 &&

                        !angular.element("#iframeaction").length > 0) {
                        angular.element(".task-comments").hide();
                        angular.element(".task-comments").removeClass("slidein");
                    }
                });
            }
        }
    }])

    // Task complete counter
    .directive("taskCompletedCount", [function () {
        return {
            restrict: 'A',
            compile: function (elem) {
                elem.html("<h5>TASKS Completed <span class='txt-color-blue'>{{taskCompleted.count}}</span></h5>" +
                    "<sparks-chart values='{{taskCompleted.value}}' offset='{{taskCompleted.tooltip}}' type='bar' color='txt-color-blue' count='{{taskCompleted.count}}'></sparks-chart>");
            }
        };
    }])

    // Task to to counter
    .directive("taskToDoCount", [function () {
        return {
            restrict: 'A',
            compile: function (elem) {
                elem.html("<h5>TASKS To Do <span class='txt-color-purple'>{{taskToDo.count}}</span></h5><sparks-chart values='{{taskToDo.value}}' offset='{{taskToDo.tooltip}}' type='bar' color='txt-color-purple' count='{{taskToDo.count}}'></sparks-chart>");
            }
        };
    }])

    // download file
    .directive("downloadFileTask", [function () {
        return {
            scope: {
                file: '=',
                taks: '='
            },
            restrict: 'E',
            replace: true,
            controller: ['$scope', function ($scope) {
                $scope.getFileOid = function (file) {
                    return typeof file.documentOid === 'undefined' ? file.oid : file.documentOid;
                };

            }],
            template: '<form method="post" action="ApproveTask/ApproveTask/Document">' +
                '<b class="pull-left col-sm-6 btnsavefile" ng-click="saveTaskFile(file)"><i class="fa fa-file"></i> ' +
                '<input type="submit" name="completeTask" value="{{file.documentName}}" class="linkBoxColor btnsavetaskfile"></b>' +
                '<input length="36" id="TaskOid" name="TaskOid" type="hidden" value="{{task.taskOid}}">' +
                '<input id="DocumentOid" name="DocumentOid" type="hidden" value="{{getFileOid(file)}}">' +
                '</form>'//<b class="pull-left col-sm-6 btnsavefile" ng-click="saveTaskFile(file)"><i class="fa fa-file"></i> {{file.documentName}}</b>
        };
    }])

    // Task details
    .directive("taskDetailsIframe", ['myAlert', '$stateParams', 'TaskFactory', '$rootScope', '$timeout', function (myAlert, $stateParams, taskFactory, $rootScope, $timeout) {
        return {
            restrict: "A",
            scope: {
                taskDetail: '=taskDetailsIframe',
                selectedTask: '=selectedTask',
                taskList: '=taskList'
            },
            link: function (scope, elem) {
                var t = scope.selectedTask;                                     // Selected Task
                var idx = parseInt(t.index);                                    // Task's index in Tasks Array
                var tmTaskSlice;

                // fn clear iframe
                var clearIframe = function() {
                  
                    // hide task detail
                    scope.taskDetail.enabled = false;

                    // clear task detail
                    jQuery(elem).empty();

                    // show 'back to list' button
                    jQuery("#taskdetailcontrol").show();
                }

                // Watch reload tasks event
                $rootScope.$on("TASKS:RELOAD", function () {
                    clearIframe();
                });
                
                // Show loading icon
                jQuery('<i class="fa-2x custom-spinning iframe-loader"></i>').appendTo(jQuery(elem));

                // Insert iframe
                jQuery("<iframe src='" + t.task.uiCode + "/" + t.task.uiCode + "?toid=" + t.task.taskOid + "'></iframe>").appendTo(jQuery(elem)).load(function () {
                    // Remove loading icon
                    jQuery(elem).find(".iframe-loader").remove();
                    // if user click button from iframe
                    if (jQuery("#iframeaction").val() !== '') {
                        scope.$apply(function () {
                            //turn off submitting state
                            scope.taskDetail.submitTask.onSubmitting = false;
                            scope.taskDetail.submitTask.hasErrors = false;

                            //if current task detail is open from sketch page then reload task list
                            if ($stateParams.toid != null) {
                                $location.url("/tasks");
                                return;
                            }

                            //alert("The data got from the iframe to the app: " + jQuery(elem).find("iframe").contents().find("body").html());
                            $timeout.cancel(tmTaskSlice);

                            // Delete task from tasks list and decrease task counter by 1
                            tmTaskSlice = $timeout(function () {
                                if (idx > -1) {
                                    scope.taskList.results.splice(idx, 1);
                                }

                                // Re-update task counter
                                taskFactory.setTaskCounter(taskFactory.counter - 1, false);

                                // alert
                                myAlert.pop("s", "Success", jQuery("#iframeaction").val() + " task successful");

                                clearIframe();
                            }, 500);
                        });
                    }
                });

                jQuery(elem).append("<input type='hidden' id='iframeaction' value='' />");
                jQuery(elem).append("<input type='hidden' id='keepTaskInList' value='0' />");
                jQuery("#taskview-iframe-loader").remove();
                jQuery(elem).append('<div id="taskview-iframe-loader" class="pre-loading" style="display: none;"><i ng-class="addClass" style="color: #2C3742;;" class="fa-2x custom-spinning"></i> <span class="preloadertext">Processing...</span></div>');

                scope.$on("$destroy", function () {
                    jQuery(elem).html('');
                });
            }
        }
    }])

    // Hide task search result
    .directive('hideTaskSearchResult', ['$document', function ($document) {
        return {
            restrict: "A",
            scope: {
                taskSearch: '=hideTaskSearchResult'
            },
            link: function (scope) {
                $document.bind('click', function (e) {
                    if (angular.element(e.target).parents(".task-search").length === 0) {
                        scope.taskSearch.onSearching = false;
                        scope.taskSearch.showSearch = false;
                    }
                });
            }
        }
    }])
;