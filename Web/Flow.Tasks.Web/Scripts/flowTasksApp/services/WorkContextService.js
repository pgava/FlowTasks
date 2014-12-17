flowTasksApp
    .service('WorkContextSrv', ['$rootScope', '$location', '$timeout', 'webStorage','ApplicationFactory',
        function ($rootScope, $location, $timeout, webStorage, applicationFactory) {
            var tm;
            return {
                isAuthenticated: function () {
                    if (webStorage.get("user") == null) {
                        webStorage.remove("user");
                        applicationFactory.clearUserData();
                        return false;
                    }
                    return true;
                },
                getCurrentUser: function () {
                    if (webStorage.get("user") != null) {
                        return webStorage.get("user");
                    }
                    //console.log($location);
                    $timeout.cancel(tm);
                    tm = $timeout(function () {
                        $location.path("/signin");
                    }, 500);
                    return {};
                },
                setCurrentUser: function (currentUser) {
                    webStorage.remove("user");
                    if (currentUser) {
                        webStorage.add("user", currentUser);
                    }
                    
                }
            };
        }
    ]);