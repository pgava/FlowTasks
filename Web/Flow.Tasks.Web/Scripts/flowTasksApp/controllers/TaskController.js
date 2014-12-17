flowTasksApp
    .controller("TaskCtrl", ['$rootScope', '$scope', 'TaskSrv', 'WorkContextSrv', 'ngTableParams', '$filter', '$timeout', '$document', 'globalConfig', 'helperSrv', '$q', '$window', 'UserSrv', '$stateParams', '$location', 'TaskFactory', 'ApplicationFactory', 'myAlert', function ($rootScope, $scope, taskSrv, workContextSrv, ngTableParams, $filter, $timeout, $document, globalConfig, helperSrv, $q, $window, userSrv, $stateParams, $location, taskFactory, applicationFactory, myAlert) {
        var currentUser = workContextSrv.getCurrentUser();                                          // Get current user object
        var username = currentUser.userName;                                                        // Get username
        var defaultPageSize = 10;                                                                   // Default page size
        var defaultPageIndex = 0;                                                                   // Default page index
        var pageSize = defaultPageSize;
        var pageIndex = defaultPageIndex;
        var loadtm = null;                                                                          // Loading timeout object

        // fn view task details
        var viewTaskDetails = function () {
            // Do request
            taskSrv.getTaskByToid($stateParams.toid)
                .then(function (dt) {
                    // open task details
                    generateTaskDetail(dt);

                    // hide preloading
                    applicationFactory.preLoading.show(false);
                }, function (dt) {
                    // Show error
                    myAlert.pop('e', 'Error!', dt);

                    // hide preloading
                    applicationFactory.preLoading.show(false);
                });
        }

        // fn open task details view
        var openTaskDetails = function (t) {
            generateTaskDetail(t);
        }

        // fn reload tasks list
        function reloadTaskList() {
            // reset page size
            pageSize = defaultPageSize;
            // reset page index
            pageIndex = defaultPageIndex;
            // reset states
            $scope.taskList.stopLoading = false;
            $scope.taskList.noResult = false;
            $scope.taskSearch.onSearching = false;
            $scope.taskSearch.showSearch = false;

            // load taks
            loadMoreTasks(true);
        }

        // fn load tasks
        function loadMoreTasks(clear) {

            // if task detail open from sketch then open task detail without loading task list
            if ($stateParams.toid != null) {
                viewTaskDetails();
                return;
            }

            // reset result
            if (clear) {
                // Clear task list
                $scope.taskList.results = [];
                // reset page size
                pageSize = defaultPageSize;
                // reset page index
                pageIndex = defaultPageIndex;
                // Remove states
                $scope.taskList.onLoading = false;
                $scope.taskList.stopLoading = false;
            }

            if (!$scope.taskList.onLoading && !$scope.taskList.stopLoading) {
                $scope.taskList.onLoading = true;

                //abort all previous request
                applicationFactory.abortPendingRequests();

                //post request get tasks
                taskSrv.getTasksByUser(username, $scope.taskSearch.query, pageSize, pageIndex++)
                    .then(function () {
                        //if there is some to load more
                        if (!$scope.taskList.stopLoading) {

                            Array.prototype.push.apply($scope.taskList.results, taskSrv.tasks);

                            if ($scope.taskList.results.length <= 0) {
                                $scope.taskList.noResult = true;
                            }

                            //if the result less than pageSize then stop loading in future
                            if (taskSrv.tasks.length < pageSize) {
                                $scope.taskList.stopLoading = true;
                            }
                        }

                        $scope.taskList.firstLoad = false;
                        $scope.taskList.onLoading = false;
                        loadtm = null;

                        // clear task blink, keep task counter number
                        taskFactory.setTaskCounter(taskFactory.counter, false);
                    },
                        function (resp) {
                            // Hide loading
                            $scope.taskList.onLoading = false;
                            if (resp.st != 401 && angular.isDefined(resp.st)) {
                                // Show error
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading tasks!');
                            }
                        })
                    .then(function () {
                        applicationFactory.preLoading.show(false);
                    });

            }
        }

        // generate task detail page
        function generateTaskDetail(t) {
            // save selected task
            $scope.selectedTask = t;
            // save scroll position
            $scope.taskDetail.scrollTop = t.scrollTop;
            // Open task details state
            $scope.taskDetail.enabled = true;
            // show task details state
            $scope.taskDetail.onShowing = true;
            // on loading task details state
            $scope.taskDetail.onLoading = false;

        }

        $scope.taskList = {
            onLoading: false,                                                                       // On tasks loading state
            stopLoading: false,                                                                     // stop task loading state
            noResult: false,                                                                        // No tasks result state
            results: [],                                                                            // Task arrays
            firstLoad: true                                                                         // First load state (Prevent callback from 'Watch')
        };

        $scope.taskSearch = {
            searchResult: [],                                                                       // Tasks search result array
            showSearch: false,                                                                      // Show task search box state
            onSearching: false,                                                                     // On task searching state
            query: '',                                                                              // Query
            showMessage: false                                                                      // Show message state
        };

        $scope.taskDetail = {
            selectedTask: null,                                                                     // Selected task
            scrollTop: 0,                                                                           // Scroll position
            enabled: false,                                                                         // Display task details state
            onAssigning: false,                                                                     // Task Assign state
            task: {},                                                                               // task object
            submitTask: {
                onSubmitting: false,                                                                // On task submit state
                hasErrors: false                                                                    // Task submit has error state
            },
            openTaskDetails: openTaskDetails                                                        // fn open task details view
        };

        // watch query change
        var taskSearchListener = $scope.$watch('taskSearch.query', function () {
            if (!$scope.taskList.firstLoad) {
                reloadTaskList();
            }
        });

        // Watch reload tasks event
        var taskReloadListener = $rootScope.$on("TASKS:RELOAD", function () {
            reloadTaskList();
        });

        // fn search result click
        $scope.chooseTaskItem = function (sitem) {
            $scope.taskList.results = [];
            $scope.taskList.noResult = false;
            $scope.taskList.results.push(sitem);
            $scope.taskSearch.showSearch = false;
            $scope.taskSearch.onSearching = false;
        };

        // Infinity scroll
        $scope.loadMoreTasks = function () {
            loadMoreTasks(false);
        };

        // back to list
        $scope.backToTasklist = function (t) {
            if ($stateParams.toid != null) {
                $location.url("/tasks");
                return;
            }

            //hide task detail
            $scope.taskDetail.enabled = false;
        };

        $scope.$on("$destroy", function () {
            // destroy listeners
            taskSearchListener();
            taskReloadListener();
        });
    }])