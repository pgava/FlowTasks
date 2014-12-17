flowTasksApp
    .factory('TaskStorageFactory', ['webStorage', function (webStorage) {

        var save = function (data) {
            webStorage.add("taskStorage", data);
        }

        var load = function () {
            return webStorage.get("taskStorage");
        }

        var clear = function () {
            webStorage.remove("taskStorage");
        }

        var taskStorageFactory = {};

        taskStorageFactory.save = save;
        taskStorageFactory.load = load;
        taskStorageFactory.clear = clear;

        return taskStorageFactory;
    }])
    .factory("TaskFactory", ['WorkContextSrv', 'TaskSrv', '$rootScope', 'UserSrv', 'ApplicationFactory', '$q', 'TaskStorageFactory', 'LoggingSrv', '$state', function (workContextSrv, taskSrv, $rootScope, userSrv, applicationFactory, $q, taskStorageFactory, loggingSrv, $state) {
        // remove task loading state
        var onLoadingTask = false;
        var onLoadingStat = false;

        var taskData = {
            counter: 0,
            previousCounter: 0,
            isBlink: false,
            reset: function () {
                taskStorageFactory.clear();
                this.counter = 0;
                this.previousCounter = 0;
                this.isBlink = false;
            },
            reBind: function () {
                var counter = taskStorageFactory.load();
                if (counter != null) {
                    this.counter = counter;
                    this.previousCounter = counter;
                }
            },
            setTaskCounter: function (number, isBlink) {
                this.previousCounter = this.counter;
                this.counter = number < 0 ? 0 : number;

                this.loadUserStat();

                if (isBlink) {
                    if (this.previousCounter != this.counter) {
                        this.isBlink = isBlink;
                    }
                } else {
                    this.isBlink = isBlink;
                }
            },
            loadTaskCounter: function (userName, blink) {
                var deferred = $q.defer();

                //Check authenticated
                if (workContextSrv.isAuthenticated()) {
                    // return if ajax is executing
                    if (onLoadingTask) {
                        deferred.resolve();
                    }
                    else {
                        // active task loading state
                        onLoadingTask = true;
                        //get topic count
                        taskSrv.getTasksCount(userName).then(function (data) {
                            // set task counter
                            taskData.setTaskCounter(data, (data == 0 || !blink) ? false : true);

                            // remove task loading state
                            onLoadingTask = false;

                            // Save task counter to local storage
                            taskStorageFactory.save(data);

                            deferred.resolve();
                        }, function (err) {
                            loggingSrv.log('loadTaskCounter error: ' + err.error);
                            //alert("Task counter Error: " + err.st);
                            // clear task counter
                            taskData.setTaskCounter(0, false);
                            // remove task loading state
                            onLoadingTask = false;
                            // redirect to sign in page
                            $state.go("signin");
                            deferred.reject();
                        });
                    }

                } else {
                    deferred.reject();
                }
                return deferred.promise;
            },
            reloadTasks: function () {
                // fire event
                $rootScope.$broadcast("TASKS:RELOAD");
            },
            statistic: {
                taskToDo: {
                    count: 0,
                    value: '',
                    tooltip: ''
                },
                taskCompleted: {
                    count: 0,
                    value: '',
                    tooltip: ''
                }
            },
            loadUserStat: function () {
                var $t = this;
                var defered = $q.defer();
                if (onLoadingStat) {
                    defered.resolve($t.statistic);
                } else {
                    onLoadingStat = true;
                    userSrv.getUserStats(applicationFactory.userData.userName).then(function (dt2) {
                        // process task completed values
                        var taskCompletedValue = [];
                        angular.forEach(dt2.tasksCompleted, function (value, key) {
                            taskCompletedValue.push(value.counter);
                        });

                        // process task completed tooltips
                        var taskCompletedTooltip = [];
                        angular.forEach(dt2.tasksCompleted, function (value, key) {
                            taskCompletedTooltip.push(value.date);
                        });

                        // bind task completed value
                        $t.statistic.taskCompleted.count = dt2.tasksCompletedCount;
                        $t.statistic.taskCompleted.value = taskCompletedValue.join(',');
                        $t.statistic.taskCompleted.tooltip = taskCompletedTooltip.join(',');

                        // process task to do values
                        var taskToDoValue = [];
                        angular.forEach(dt2.tasksToDo, function (value, key) {
                            taskToDoValue.push(value.counter);
                        });

                        // process task completed tooltips
                        var taskToDoTooltip = [];
                        angular.forEach(dt2.tasksToDo, function (value, key) {
                            taskToDoTooltip.push(value.date);
                        });

                        // bind task completed value
                        $t.statistic.taskToDo.count = dt2.tasksToDoCount;
                        $t.statistic.taskToDo.value = taskToDoValue.join(',');
                        $t.statistic.taskToDo.tooltip = taskToDoTooltip.join(',');

                        defered.resolve($t.statistic);
                    }, function (err) {
                        // error goes here
                        defered.reject();
                    });
                    onLoadingStat = false;

                }
                
                return defered.promise;
            }
        }

        return taskData;
    }])