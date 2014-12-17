flowTasksApp
    .service('UserSrv', ['$http', '$q', 'helperSrv', 'webStorage','ApplicationFactory',
        function ($http, $q, helperSrv, webStorage, applicationFactory) {

            // Get user by Name
            this.getUserByName = function (name, abortable) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/users', params: { name: name }, cache: false, headers: { 'Abortable': abortable } })
                   .success(function (dt, st) {
                       deferred.resolve(dt);
                   }, function (err) {
                       deferred.reject(err);
                   });
                // return promise
                return deferred.promise;
            };

            // authorizing user
            this.authorize = function (username, password, remember) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                // PG. made this change
                $http({ method: 'POST', url: 'account/logon', data: { userName: username, password: password, rememberMe: remember, returnUrl: '' }, cache: false })
                    .success(function (dt, st) {
                        deferred.resolve(dt);
                    }, function (err) {
                        deferred.reject(err);
                    });
                // return promise
                return deferred.promise;
            };

            // log out
            this.logout = function () {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/users', cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Update user info
            this.updateUserInfo = function (userInfo) {
                //Create new object
                var userObject = {};
                //Copy angular model to new object to prevent binding new data
                angular.copy(userInfo, userObject);
                //Check if user birthday not null
                if (userObject.BirthdayStr !== '') {
                    //convert formated date string to date object
                    var birthDay = helperSrv.convertFormatedStringToDateObject(userObject.BirthdayStr);
                    //convert date object to server date
                    userObject.BirthdayStr = helperSrv.convertDateObjectToServerDate(birthDay);
                }
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'PATCH', url: 'api/users', data: userObject, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Update user password
            this.updateUserPassword = function (userPassword) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'POST', url: 'api/users/password/' + userPassword.name + '/' + userPassword.oldp + '/' + userPassword.newp, data: userPassword, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Remove following
            this.removeFollowing = function (follower) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'DELETE', url: 'api/users/follows/' + follower.userName + '/' + follower.followerName, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Follow user
            this.followUser = function (follower) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'POST', url: 'api/users/follows/' + follower.userName + '/' + follower.followerName, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Upload avatar
            this.uploadAvatar = function (url, avatar) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({
                    method: 'POST',
                    url: url,
                    data: avatar,
                    cache: false,
                    headers: { 'Content-Type': undefined },
                    transformRequest: function (data) {
                        if (typeof FormData != 'undefined') {
                            var fd = new FormData();
                            angular.forEach(data, function (value, key) {
                                fd.append(key, value);
                            });
                            return fd;
                        }
                    }
                })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Search users
            this.searchUser = function (nameToSearch) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/users', params: nameToSearch, cache: false })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Get user stats
            this.getUserStats = function (user) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/users/stats/' + user, cache: false, headers: { 'Abortable': false } })
                .success(function (dt, st) {
                    deferred.resolve(dt);
                }, function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Check if user is authenticated
            this.isAuthenticated = function () {
                if (webStorage.get("user") == null) {
                    webStorage.remove("user");
                    applicationFactory.clearUserData();
                    return false;
                }
                return true;
            }

            // Get user local data
            this.getUserLocalData = function () {
                return webStorage.get("user");
            };
        }
    ]);