flowTasksApp
    .service('ReportService', ['$q', '$http', function ($q, $http) {

        // Get user tasks
        this.getUserTasks=function(data) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({
                method: 'GET',
                url: 'api/reports/usertasks',
                params: data,
                dataType: 'json',
                cache: false
            }).success(function(resp) {
                // success
                deferred.resolve(resp);
            }).error(function(err) {
                // reject promise
                deferred.reject(err);
            });

            // Return promise
            return deferred.promise;
        }

        // Get user tasks count
        this.getUserTaskCount = function (data) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({
                method: 'GET',
                url: 'api/reports/taskcount',
                params: data,
                dataType: 'json',
                cache: false
            }).success(function (resp) {
                // success
                deferred.resolve(resp);
            }).error(function (err) {
                // reject promise
                deferred.reject(err);
            });

            // Return promise
            return deferred.promise;
        }

        // Get tasks
        this.getTasks = function (data) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({
                method: 'GET',
                url: 'api/reports/tasktime',
                params: data,
                dataType: 'json',
                cache: false
            }).success(function (resp) {
                // success
                deferred.resolve(resp);
            }).error(function (err) {
                // reject promise
                deferred.reject(err);
            });

            // Return promise
            return deferred.promise;
        }

        // Get workflows
        this.getWorkflows = function (data) {
            // Make promise
            var deferred = $q.defer();

            // Do request
            $http({
                method: 'GET',
                url: 'api/reports/workflowtime',
                params: data,
                dataType: 'json',
                cache: false
            }).success(function (resp) {
                // success
                deferred.resolve(resp);
            }).error(function (err) {
                // reject promise
                deferred.reject(err);
            });

            // Return promise
            return deferred.promise;
        }

    }])