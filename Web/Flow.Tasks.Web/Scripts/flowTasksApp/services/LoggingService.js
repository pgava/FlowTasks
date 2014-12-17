flowTasksApp
    .service('LoggingSrv', ['$http', '$q', '$window', 'UserSrv',
    function ($http, $q, $window, userSrv) {

            // Get user by Name
            this.log = function (message) {

                var user = userSrv.getUserLocalData();
                var userName = "";
                if (user) {
                    userName = user.userName;
                }
                var userAgent = $window.navigator.userAgent;

                message = userAgent + ' - ' + userName + ' - ' + message;

                // Make promise
                var deferred = $q.defer();
                $http({ method: 'POST', url: 'api/logging', data: {message: message}, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };           
        }
    ]);