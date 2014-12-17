var flowTasksApp = angular.module('flowTasksApp', ["ngResource", "ui.router", 'ngSanitize', 'ui.utils', 'infinite-scroll', 'ngTable', 'webStorageModule', 'ngTouch', 'myAlert', 'googlechart']);

flowTasksApp.run(['$rootScope', '$timeout', '$state', 'WorkContextSrv', 'webStorage', 'UserSrv', '$interval', '$window', '$location', 'ApplicationFactory', 'TopicFactory', 'TaskFactory', function ($rootScope, $timeout, $state, workContextSrv, webStorage, userSrv, $interval, $window, $location, applicationFactory, topicFactory, taskFactory) {


    //---------------------------------------------------
    // For unit testing avoid to execute this method
    if (workContextSrv.isAuthenticated()) {
        var u = workContextSrv.getCurrentUser();
        if (u.userName === "TestUser") {
            return;
        }
    }
    //---------------------------------------------------

    $rootScope.onPreloading = true;

    if (!/^(.*)\/$/i.test(location.pathname)) {
        if (location.href.indexOf('#') > -1) {
            location.href = location.href.split('#')[0] + "/#" + location.href.split('#')[1];
        } else {
            location.href = location.href + "/";
        }
    }

    //State change handler
    $rootScope.$on('$stateChangeStart',
        function (event, toState, toParams, fromState, fromParams) {
            if (!/^(.*)\/$/i.test(location.pathname)) {
                event.preventDefault();
            }
            else if (toState.name === 'signin' && workContextSrv.isAuthenticated()) {
                $state.go("home");
            }
            else if (angular.isDefined(toState.authorize) && toState.authorize && !workContextSrv.isAuthenticated()) {
                // prevent default action
                event.preventDefault();

                // hide menu
                applicationFactory.showHiddenMenu();

                // go to signin page
                $state.go("signin");
            } else {
                // fire chagingPage event
                $rootScope.$broadcast("chagingPage");

                // save current page name
                applicationFactory.page.current = toState.name;

                // loading effect
                applicationFactory.preLoading.show(true);

            }
        }
    );
    
    // Start interval loading topic counter and task counter
    $timeout(function () {
        // load topic counter
        topicFactory.loadTopicCounter(applicationFactory.userData.userName);
        $interval(function () {
            // load topic counter
            topicFactory.loadTopicCounter(applicationFactory.userData.userName, true);
        }, 120000);

        // load task counter
        taskFactory.loadTaskCounter(applicationFactory.userData.userName, true);
        $interval(function () {
            // load task counter
            taskFactory.loadTaskCounter(applicationFactory.userData.userName, true);
        }, 120000);
    }, 2000);

    //Get user info if refresh browser
    if (userSrv.isAuthenticated()) {
        //get user from local storage
        var currentUser = userSrv.getUserLocalData();
        
        //get user from server
        userSrv.getUserByName(currentUser.userName, false).then(function (dt) {
            //set new user info
            workContextSrv.setCurrentUser(dt);

            applicationFactory.updateUserData({
                userName: dt.userName,
                email: dt.email,
                firstName: dt.firstName,
                lastName: dt.lastName,
                photoPath: dt.photoPath,
                pages: dt.userPages,
                follower: dt.follower,
                followerCount: dt.followerCount,
                following: dt.following,
                followingCount: dt.followingCount
            });

            //get user stat
            taskFactory.loadUserStat().then(function () {
                $rootScope.onGlobalLoading = false;
            });
        });

    } else {
        //redirect to signin page
        $location.path('/signin');
    }

    //Loader
    $rootScope.totalCall = 0;
    $rootScope.$on('LOADER:SHOW', function () {
        $rootScope.showLoading = true;
        $rootScope.totalCall = $rootScope.totalCall + 1;
    });

    $rootScope.$on('LOADER:HIDE', function () {
        $rootScope.totalCall = $rootScope.totalCall - 1;
        if ($rootScope.totalCall == 0) {
            $rootScope.showLoading = false;
        }
    });

}])

// Unhandle errors and Authorized
.factory('authHttpResponseInterceptor', ['$q', '$location', function ($q, $location) {
    return {
        response: function (response) {
            if (response.status === 401) {
                //console.log("Response 401");
            }
            return response || $q.when(response);
        },
        responseError: function (rejection) {
            console.log(rejection);
            if (rejection.status === 401) {
                $location.path('/signin');
            }
            return $q.reject(rejection);
        }
    };
}])

// Http Request interceptor - Abort Pending requests
.factory('abortHttpResponseInterceptor', ['$q', '$rootScope', function ($q, $rootScope) {
    return {
        request: function (config) {
            var canceler = $q.defer();
            config.timeout = canceler.promise;
            $rootScope.$on("abortPendingRequest", function (event, params) {
                if (params) {
                    canceler.resolve();
                } else {
                    //don't abort html request, header with 'abortable' is false
                    if (/(\.html)$/g.test(config.url) || (angular.isDefined(config.headers.Abortable) && config.headers.Abortable === false)) {

                    } else {
                        canceler.resolve();
                    }
                }
                
            });
            return config || $q.when(config);
        }
    };
}])

.config(['$httpProvider', function ($httpProvider) {
    //Http Intercpetor to check auth failures for xhr requests
    $httpProvider.interceptors.push('authHttpResponseInterceptor');

    //add abort event to xhr request
    $httpProvider.interceptors.push('abortHttpResponseInterceptor');
}])
.constant('globalConfig', {
    allowedExtension: 'jpg, jpeg , png, rar, zip, doc, docx, xls, xlsx, ppt, pptx, pdf',
    regexAllowedExtension: /(\.|\/)(jpg|jpeg|png|rar|zip|doc|docx|xls|xlsx|ppt|pptx|pdf)$/i,
    regexAllowedImageExtension: /(\.|\/)(jpg|jpeg|png)$/i,
    allowedTaskDocumentExtension: 'xamlx',
    regexAllowedTaskDocumentExtension: /(\.|\/)(xamlx)$/i,
    maxFiles: 3,
    fileSize: 522752,
    imageSize: 522752,
    dateFormat: !angular.isDefined(appConfig) ? 'mm/dd/yy' : appConfig.dateFormat,
    momentDateFormat: !angular.isDefined(appConfig) ? 'MM/DD/YYYY' : appConfig.momentDateFormat,
    momentServerDateFormat: !angular.isDefined(appConfig) ? 'YYYY-MM-DD' : appConfig.momentServerDateFormat
})
.service('globalSrv', [function () {

}])
.service('helperSrv', ['globalConfig', function (globalConfig) {
    //get ie version

    this.isIe = function () {
        var myNav = navigator.userAgent.toLowerCase();
        if (/.net(.*?)\;/g.test(myNav) && /rv\:(\d+).*\)/g.test(myNav)) {
            return parseInt(/rv\:(\d+).*\)/g.exec(myNav)[1]);
        }
        return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
        //return 9;
    };

    this.supportDragDrop = function () {
        return !jQuery('html').hasClass('notsupportdragdrop');
        //return false;
    };

    this.convertDateObjectToFormatedString = function (date) {
        return moment(date).format(globalConfig.momentDateFormat);
    };

    this.convertFormatedStringToDateObject = function (dateString) {
        return moment(dateString, globalConfig.momentDateFormat).toDate();
    };

    this.convertServerDateToDateObject = function (serverDate) {
        return new Date(serverDate);
    };

    this.convertDateObjectToServerDate = function (date) {
        return moment(date).format(globalConfig.momentServerDateFormat);
    };

}]);
