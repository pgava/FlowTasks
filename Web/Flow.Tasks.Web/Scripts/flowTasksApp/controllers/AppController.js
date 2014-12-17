flowTasksApp
    // App Controller
    .controller('AppCtrl', ['$rootScope',
        "$scope", "WorkContextSrv", "$state", 'TopicSrv', 'TaskSrv', 'TaskFactory', 'TopicFactory', 'ApplicationFactory', '$timeout', function ($rootScope, $scope, workContextSrv, $state, topicSrv, taskSrv, taskFactory, topicFactory, applicationFactory, $timeout) {

            // Set page title
            $scope.pageTitle = applicationFactory.page;


            // application scope
            $scope.application = {
                taskData: taskFactory,                              // Task data
                topicData: topicFactory,                            // Topic data
                // fn go to Task page (if current page is Task already, then execute reload tasks)
                goToTask: function () {
                    // if current page is task page
                    if ($state.current.name === 'task') {
                        // load task counter
                        //taskFactory.loadTaskCounter(applicationFactory.userData.userName, false).then(function() {
                        //    // trigger reload tasks in task page
                        //    taskFactory.reloadTasks();
                        //});

                        taskFactory.reloadTasks();
                    } else {
                        // Go to task page
                        $state.go("task");
                        taskFactory.setTaskCounter(taskFactory.counter, false);
                    }
                },
                // fn go to Home page (if current page is Home already, then execute reload topics)
                goToHome: function () {
                    // if current page is home or default then reload topics
                    if ($state.current.name === 'home' || $state.current.name === 'default') {
                        // load task counter
                        //topicFactory.loadTopicCounter(applicationFactory.userData.userName, false).then(function () {
                        //    // clear topic count
                        //    topicFactory.setTopicCounter(0, false);

                        //    // reload topics in topic page (home page)
                        //    topicFactory.reloadTopics();
                        //});

                        topicFactory.reloadTopics();
                    } else {
                        // go to home page
                        $state.go("home");
                    }

                    // clear topic count
                    topicFactory.setTopicCounter(0, false);
                },
                userData: applicationFactory.userData,               // User data
            }

        }
    ])

    // Login Controller
    .controller('LoginCtrl', ['$rootScope',
        "$scope", "WorkContextSrv", "$state", "UserSrv", 'ApplicationFactory', 'TaskFactory', 'TopicFactory', 'myAlert', function ($rootScope, $scope, workContextSrv, $state, userSrv, applicationFactory, taskFactory, topicFactory, myAlert) {
            $scope.login = {
                errorMessage: "",                                           // Error message
                showError: false,                                           // Show error State
                onSubmitting: false,                                        // On submit state
                user: { userName: '', password: '', remember: false },      // User view model
                // fn validate loginform form
                validate: function (field, type) {
                    if (type === 'required') {
                        return ($scope.showError && (typeof $scope.loginform[field].$viewValue === 'undefined' || $scope.loginform[field].$viewValue === ''));
                    }
                    return false;
                },
                // fn login
                doLogin: function (user) {
                    $scope.login.showError = true;
                    $scope.login.errorMessage = "";
                    if (user.userName != '' && user.password != '' && $scope.loginform.$valid) {

                        // show loading
                        $scope.login.onSubmitting = true;

                        // Do login request
                        userSrv.authorize(user.userName, user.password, user.remember).then(function (data, status) {
                            // hide loading
                            $scope.login.onSubmitting = false;
                            // if success
                            if (data == true) {
                                $scope.login.onSubmitting = true;
                                // Do request get user info
                                userSrv.getUserByName(user.userName).then(function (dt) {
                                    // set data to service
                                    workContextSrv.setCurrentUser(dt);

                                    // set data to factory
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

                                    // Fire user loggin in event
                                    $rootScope.$broadcast("USER:LOGIN");

                                    // Redirect to home page
                                    $state.go("home");

                                },
                                function () {
                                    // Show error message
                                    myAlert.pop('e', 'Error!', 'There was an error occurred while loading user information');
                                    $state.go("signin");
                                });
                            } else {
                                $scope.login.errorMessage = "The username or password provided is incorrect!";
                                $scope.login.onSubmitting = false;
                                $scope.login.showError = true;
                            }
                        }, function (data) {
                            $scope.login.showError = true;
                            $scope.login.errorMessage = data.message;
                            $scope.login.onSubmitting = false;
                        });
                    }
                }
            }
        }
    ]);