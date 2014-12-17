flowTasksApp
    .controller('DashboardCtrl', [
        "$scope", '$rootScope', 'ngTableParams', '$filter', 'DashboardSrv', '$q', '$timeout', 'globalConfig', '$stateParams', 'ApplicationFactory', 'TaskFactory', 'myAlert', function ($scope, $rootScope, ngTableParams, $filter, dashboardSrv, $q, $timeout, globalConfig, $stateParams, applicationFactory, taskFactory, myAlert) {

            // dashboard model
            $scope.dashboard = {
                workflow: {
                    formatDate: globalConfig.dateFormat,                                // Date format
                    selectedWorkflow: [],                                               // Selected workflows array
                    expandedWorkflows: [],                                              // Expanded workflows array
                    data: [],                                                           // Workflows array
                    // Filter data is parent only
                    dataFilter: function (item) {
                        return item.parentid === '' || item.parentid === null;
                    },
                    total: 0,                                                           // Workflows total count
                    // Search view model
                    search: {
                        workflowCodes: ['All'],                                         // Workflows codes
                        pageSizes: [10, 50, 100, 500]                                   // Page sizes
                    },
                    // Filter view model
                    filter: {
                        woid: '',                                                       // Workflow id
                        wcode: 'All',                                                   // Workflow code
                        isActive: true,                                                 // Active
                        start: '',                                                      // Start date
                        end: '',                                                        // End date
                        pageSize: 10                                                    // Page size
                    },
                    pageControl: {},                                                    // Page control (Pagination)
                    pageIndex: 0,                                                       // Page index
                    onFiltering: false,                                                 // On filtering state
                    onLoading: false,                                                   // On loading state
                    onAction: false,                                                    // On do action state (Terminate, Restart or Delete)
                    onViewDetail: false,                                                // On loading workflow details
                    detail: {},                                                         // Workflow details view model
                    showMessage: false,                                                 // Show message state
                    hasErrors: false,                                                   // Has error state
                    hasWarning: false,                                                  // Has Warning state
                    message: '',                                                        // Message (Error, success)
                    totalWorkflowInPage: 0                                              // Total workflow in page
                },
                firstLoad: true                                                         // First load state (Prevent callback from 'Watch' from first load)
            };

            // fn get workflows
            function getWorkflows(reDraw, hideMessage) {
                //show overlay
                $scope.dashboard.workflow.onLoading = true;

                if (hideMessage) {
                    //hide message
                    $scope.dashboard.workflow.showMessage = false;
                }

                //abort all previous request
                applicationFactory.abortPendingRequests();

                // clear expanded workflows
                $scope.dashboard.workflow.expandedWorkflows = [];

                //get data
                dashboardSrv.getWorkflows($scope.dashboard.workflow.filter, $scope.dashboard.workflow.pageIndex + 1)
                    .then(function (dt) {
                        //bind data to model
                        $scope.dashboard.workflow.data = dt.workflows;
                        $scope.dashboard.workflow.totalWorkflowInPage = 0;
                        angular.forEach(dt.workflows, function (dta, idx) {
                            if (dta.parentid === '' || dta.parentid === null) {
                                $scope.dashboard.workflow.totalWorkflowInPage = $scope.dashboard.workflow.totalWorkflowInPage + 1;
                            }
                        });


                        //hide loading
                        $scope.dashboard.workflow.onLoading = false;

                        if (reDraw) {
                            //count pagination
                            $scope.dashboard.workflow.total = Math.ceil(dt.totalWorkflows / $scope.dashboard.workflow.filter.pageSize);
                            //re-draw pagination
                            $timeout(function () {
                                $scope.dashboard.workflow.pageControl.reDraw($scope.dashboard.workflow.pageIndex);
                            }, 500);
                        }

                        //bind workflow codes
                        if (dt.workflowCodes.length) {
                            //clear workflow codes
                            $scope.dashboard.workflow.search.workflowCodes = [];
                            //add 'All' workflow code
                            $scope.dashboard.workflow.search.workflowCodes.push('All');
                            //add workflow code return
                            dt.workflowCodes.forEach(function (dta) {
                                $scope.dashboard.workflow.search.workflowCodes.push(dta);
                            });
                        } else {
                            //clear workflow codes and add 'All' workflow code
                            $scope.dashboard.workflow.search.workflowCodes = ['All'];
                            //set selected workflow code
                            $scope.dashboard.workflow.filter.wcode = 'All';
                        }


                        //hide preloading
                        applicationFactory.preLoading.show(false);

                        //remove firstload state
                        $scope.dashboard.firstLoad = false;

                        //filtering state to prevent pageIndex watching
                        $scope.dashboard.workflow.onFiltering = false;
                    }, function (dt, st) {
                        if (dt !== null && angular.isDefined(dt)) {
                            //show the message
                            $scope.dashboard.workflow.showMessage = true;
                            //show error message
                            $scope.dashboard.workflow.hasErrors = true;
                            //bind message
                            $scope.dashboard.workflow.message = (angular.isDefined(dt.exceptionMessage) ? dt.exceptionMessage : dt.message);
                        }
                        // hide loading
                        $scope.dashboard.workflow.onLoading = false;
                    });
            }

            // change page
            $scope.$watch("dashboard.workflow.pageIndex", function () {
                if (!$scope.dashboard.firstLoad && !$scope.dashboard.workflow.onFiltering) {
                    //reload table
                    getWorkflows(false, true);
                }
            });

            // watch user typing name
            $scope.$watch("dashboard.workflow.filter", function () {
                //filtering state to prevent pageIndex watching
                $scope.dashboard.workflow.onFiltering = true;

                //hide message
                $scope.dashboard.workflow.showMessage = false;

                //hide warning message
                $scope.dashboard.workflow.hasWarning = false;

                //set page index to 1
                $scope.dashboard.workflow.pageIndex = 0;
                var valid = true;

                //convertToServerDate
                //validate workflow format
                if ($scope.dashboard.workflow.filter.woid !== '' && !/\w{8}\-\w{4}\-\w{4}\-\w{4}\-\w{12}/g.test($scope.dashboard.workflow.filter.woid)) {
                    return;
                }

                //reload table
                getWorkflows(true, true);

                //remove firstload state
                $scope.dashboard.firstLoad = false;
            }, true);

            // fn get workflows child by parent
            $scope.getWorkflowChild = function (wfid) {
                var childs = [];
                for (var i = 0; i < $scope.dashboard.workflow.data.length; i++) {
                    if ($scope.dashboard.workflow.data[i].parentid !== null && $scope.dashboard.workflow.data[i].parentid === wfid) {
                        childs.push($scope.dashboard.workflow.data[i]);
                    }
                }
                return childs;
            };

            // fn expand child
            $scope.expandChild = function (w) {
                if ($scope.isHaveChild(w)) {
                    if ($scope.dashboard.workflow.expandedWorkflows.indexOf(w.wfid) > -1) {
                        //remove selected workflow from list
                        $scope.dashboard.workflow.expandedWorkflows.splice($scope.dashboard.workflow.expandedWorkflows.indexOf(w.wfid), 1);
                    } else {
                        //add expanded workflow to list
                        $scope.dashboard.workflow.expandedWorkflows.push(w.wfid);
                    }
                }
            };

            // fn check workflow have child or not
            $scope.isHaveChild = function (w) {
                for (var i = 0; i < $scope.dashboard.workflow.data.length; i++) {
                    if ($scope.dashboard.workflow.data[i].parentid === w.wfid) {
                        return true;
                    }
                }
                return false;
            };

            // check is expanded
            $scope.isExpanded = function (w) {
                return $scope.dashboard.workflow.expandedWorkflows.indexOf(w.wfid) > -1;
            };

            // check selected state
            $scope.isSelected = function (w) {
                return $scope.dashboard.workflow.selectedWorkflow.indexOf(w.wfid) > -1;
            };

            // fn select workflow
            $scope.selectWorkflow = function (w, selected) {
                if (!selected) {
                    if ($scope.dashboard.workflow.selectedWorkflow.indexOf(w.wfid) > -1) {
                        //remove selected workflow from list
                        $scope.dashboard.workflow.selectedWorkflow.splice($scope.dashboard.workflow.selectedWorkflow.indexOf(w.wfid), 1);
                    }
                } else {
                    //add selected workflow to list
                    $scope.dashboard.workflow.selectedWorkflow.push(w.wfid);
                }
            };

            //Restart, Teminate, Delete action
            $scope.doAction = function (action) {

                //hide the message
                $scope.dashboard.workflow.showMessage = false;
                //hide warning message
                $scope.dashboard.workflow.hasWarning = false;
                //if user not select any workflow
                if (!$scope.dashboard.workflow.selectedWorkflow.length) {
                    //show warning message
                    $scope.dashboard.workflow.showMessage = true;
                    $scope.dashboard.workflow.hasWarning = true;
                    $scope.dashboard.workflow.message = 'Please select at least 1 workflow';
                    return;
                }

                //show overlay
                $scope.dashboard.workflow.onAction = true;

                dashboardSrv.doWorkflow(action, $scope.dashboard.workflow.selectedWorkflow)
                    .then(function (dt) {
                        //Display message
                        $scope.dashboard.workflow.showMessage = true;
                        if (!dt.length) {
                            //console.log($scope.dashboard.workflow.totalWorkflowInPage);
                            //console.log($scope.dashboard.workflow.selectedWorkflow.length);
                            if ($scope.dashboard.workflow.totalWorkflowInPage <= $scope.dashboard.workflow.selectedWorkflow.length && action === 'Delete') {
                                $scope.dashboard.firstLoad = true;
                                $scope.dashboard.workflow.pageIndex = $scope.dashboard.workflow.pageIndex - 1 <= 0 ? 0 : $scope.dashboard.workflow.pageIndex - 1;
                            }
                            //clear selected workflow
                            $scope.dashboard.workflow.selectedWorkflow = [];

                            //reload table
                            getWorkflows(true, false);

                            //Re-update task counter
                            taskFactory.loadTaskCounter(applicationFactory.userData.userName, false);

                            $scope.dashboard.workflow.hasErrors = false;
                            $scope.dashboard.workflow.message = action + ' selected workflows successful';
                        } else {
                            $scope.dashboard.workflow.hasErrors = true;
                            var errorMessages = [];
                            for (var i = 0; i < dt.length; i++) {
                                var error = dt[i];
                                errorMessages.push({ id: error.id, message: error.message });
                            }
                            $scope.dashboard.workflow.messages = errorMessages;
                        }
                        //hide overlay
                        $scope.dashboard.workflow.onAction = false;
                    }, function (dt) {
                        //hide overlay
                        $scope.dashboard.workflow.onAction = false;
                        if (dt !== null && angular.isDefined(dt)) {
                            //Display message
                            $scope.dashboard.workflow.showMessage = true;

                            $scope.dashboard.workflow.hasErrors = true;
                            $scope.dashboard.workflow.message = angular.isDefined(dt.exceptionMessage) ? dt.exceptionMessage : dt.message;
                        }
                    });
            };

            //view detail
            $scope.viewWorkflowDetail = function (w) {
                //hide message
                $scope.dashboard.workflow.showMessage = false;

                $scope.dashboard.workflow.onLoading = true;
                dashboardSrv.getWorkflowDetail(w.wfid).then(function (dt) {
                    //bind detail
                    $scope.dashboard.workflow.detail = dt;
                    //add wfid field
                    $scope.dashboard.workflow.detail.wfid = w.wfid;

                    $scope.dashboard.workflow.onViewDetail = true;
                    $scope.dashboard.workflow.onLoading = false;
                }, function (dt, st) {
                    $scope.dashboard.workflow.onLoading = false;
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading workflow details');
                });
            };

            //back to list
            $scope.workflowBackTolist = function () {
                $scope.dashboard.workflow.detail = {};
                $scope.dashboard.workflow.onViewDetail = false;
            };

            //refresh workflows
            $scope.refreshWorkflowsList = function () {
                //prevent pageindex changing event fire
                $scope.dashboard.firstLoad = true;

                //reset pageindex to 1
                $scope.dashboard.workflow.pageIndex = 0;

                //reload table
                getWorkflows(true, true);
            };

            $scope.isDocumentType = function (property) {
                return property.type === 'FlowDoc';
            }

        }
    ]);