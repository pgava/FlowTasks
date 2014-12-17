flowTasksApp
    .service('HolidaySrv', ['$http', '$q',
        function ($http, $q) {

            // Get user holidays
            this.getUserHolidays = function (userName, year) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/users/holiday/' + userName, params: { year: year }, cache: false })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });

                // return promise
                return deferred.promise;

            };

            // submit holidays
            this.applyHolidays = function (userName, dates, timeout) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'POST', url: 'api/users/holiday/' + userName, data: dates, cache: false, headers: { 'Content-Type': 'application/json; charset=utf-8' }, timeout: timeout })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });

                // return promise
                return deferred.promise;
            };

            // start holiday workflow
            this.startHolidayWorkflow = function (data, params) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'PATCH', url: 'api/workflows/start/', data: data, params: params, cache: false })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });

                // return promise
                return deferred.promise;
            };

        }
    ]);