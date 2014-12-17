flowTasksApp
    .service('TaskSrv', ['$http', '$q',
    function ($http, $q) {
        var _tasks = [];
        var _comments = [];

        // Tasks list
        this.tasks = _tasks;

        // Comments list
        this.comments = _comments;

        // Get Tasks counter
        this.getTasksCount = function (user) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({ method: 'GET', url: 'api/tasks', params: { toid: '', user: user }, cache: false, headers: { 'Abortable': false } })
            .success(function (data) {
                deferred.resolve(data);
            }).error(function (err,st) {
                deferred.reject({ error: err, st: st });
            });
            // retun promise
            return deferred.promise;
        };

        // Get Tasks by user
        this.getTasksByUser = function (user, searchFor, pageSize, pageIndex) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({ method: 'GET', url: 'api/tasks', params: { user: user, pageIndex: pageIndex, pageSize: pageSize, searchFor: searchFor }, cache: false })
                .then(function (result) {
                    angular.copy(result.data, _tasks);
                    deferred.resolve();
                },
                    function (error, st) {
                        deferred.reject({ error: error, st: st });
                    });

            return deferred.promise;
        };

        // Get task by id
        this.getTaskByToid = function (taskOid) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({ method: 'GET', url: 'api/tasks/' + taskOid, cache: false })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });
            // Return promise
            return deferred.promise;
        };

        // Get comments by task
        this.getCommentsForTask = function (taskOid) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({ method: 'POST', url: 'api/comments/' + taskOid, cache: false }).success(function (data) {
                deferred.resolve(data);
            }).error(function (err) {
                deferred.reject(err);
            });
            // Return promise
            return deferred.promise;
        };

        // Task operation
        this.taskOperation = function (toid, op, userName, result, message) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({
                method: 'PATCH',
                url: 'api/tasks/' + toid + '/' + op,
                data: { 'acceptedBy': userName },
                params: { result: result, message: message },
                headers: { 'Content-Type': 'application/json' },
                cache: false
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (err) {
                deferred.reject(err);
            });
            // Return promise
            return deferred.promise;
        };
    }
    ]);