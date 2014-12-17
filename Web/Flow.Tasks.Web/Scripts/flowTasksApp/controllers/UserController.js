flowTasksApp
    .controller('UserCtrl', ['$rootScope',
        "$scope", "WorkContextSrv", "ApplicationFactory", function ($rootScope, $scope, workContextSrv, applicationFactory) {

            // hide pre-loading
            applicationFactory.preLoading.show(false);

            // set active tab
            $scope.currentTab = "info";

            // fn change tab function
            $scope.changeTab = function (tab) {
                $scope.currentTab = tab;
            };

            // check current tab
            $scope.isTab = function (tab) {
                return $scope.currentTab == tab;
            };

        }
    ])

    // Profile page
    .controller('UserProfileCtrl', ['$scope', 'globalConfig', 'helperSrv', 'UserSrv', 'WorkContextSrv', '$q', 'ApplicationFactory', 'myAlert', function ($scope, globalConfig, helperSrv, userSrv, workContextSrv, $q, applicationFactory, myAlert) {
        var currentUser = workContextSrv.getCurrentUser();

        // convert birtday format
        var birtdayFormated = currentUser.birthday === null ? '' : helperSrv.convertDateObjectToFormatedString(new Date(currentUser.birthday));

        // create user view model
        $scope.currentUser = {
            UserName: currentUser.userName,
            //Birthday: birtdayFormated,
            BirthdayStr: birtdayFormated,
            Email: currentUser.email,
            FirstName: currentUser.firstName,
            LastName: currentUser.lastName,
            Gender: currentUser.gender,
            Note: currentUser.note,
            Phone: currentUser.phone,
            Position: currentUser.position,
            Department: currentUser.department,
            PhotoPath: currentUser.photoPath,
        };
        
        $scope.birthdayFormat = globalConfig.dateFormat;                                    // Date Format for datepicker
        $scope.maxBirthdateYear = new Date().getFullYear() - 10;                            // Max year for Datepicker
        $scope.saveInfoSuccessful = false;                                                  // Save info successful state
        $scope.saveInfoShowMessage = false;                                                 // Show message state
        $scope.saveInfoMessage = '';                                                        // Messages

        // fn save user info
        $scope.saveInfo = function (userModel) {
            // show indicator
            $scope.onSubmitting = true;
            // Do request update user
            userSrv.updateUserInfo(userModel)
                .then(function () {
                    // Do request get user by user name
                    userSrv.getUserByName(userModel.UserName)
                        .then(function (dt) {
                            //console.log(dt);
                            workContextSrv.setCurrentUser(dt);

                            // re-update user info
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

                            //hide indicators
                            $scope.onSubmitting = false;
                            $scope.saveInfoSuccessful = true;
                            $scope.saveInfoShowMessage = true;
                            $scope.saveInfoMessage = 'Your info has been saved successful!';
                        }, function () {
                            $scope.onSubmitting = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while getting user info');
                        });
                }, function (data, status) {

                    //hide indicator
                    $scope.onSubmitting = false;

                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while updating user info');
                });
        };

        // upload config
        var deferred = $q.defer();
        $scope.avatar = {
            // files array
            filesList: [],
            // Promise
            defer: deferred,
            // Request url
            postUrl: "api/users/avatar/" + currentUser.userName + "?name=" + currentUser.userName,
            // clear state
            isClear: false
        }

        // on avatar upload complete
        deferred.promise.then(function (data) {
            // Re-update current user avatar
            $scope.currentUser.PhotoPath = data.replace(/^~\//g, '');
        });

    }])

    // Following page
    .controller('UserFollowingCtrl', ['$scope', 'ApplicationFactory', 'ngTableParams', 'WorkContextSrv', 'UserSrv', '$filter', '$rootScope', 'myAlert', function ($scope, applicationFactory, ngTableParams, workContextSrv, userSrv, $filter, $rootScope, myAlert) {

        // declare following list
        var followingList = [];                                             // Followings array

        // filter model
        $scope.followingFilter = { name: '' };                              // Filter model

        // hide indicator
        $scope.onLoadingUser = false;                                       // On user loading state

        // setup table
        $scope.followingTable = new ngTableParams({                         // ng-table
            page: 1,            // show first page
            count: 10,          // count per page
            filter: {
                //userName: 'M'       // initial filter
            }
        }, {
            counts: [],
            total: followingList.length, // length of data
            getData: function ($defer, params) {
                // use build-in angular filter
                var orderedData = params.filter() ?
                       $filter('filter')(followingList, params.filter()) :
                       followingList;
                params.total(orderedData.length); // set total for recalc pagination
                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
            },
            $scope: { $data: {} }
        });

        // handle 'reloadFollowingTable' event
        $scope.$on("reloadFollowingTable", function (e, params) {
            //abort all previous request
            applicationFactory.abortPendingRequests();

            if ($scope.followingFilter.name === '') {
                $scope.onLoadingUser = false;
                followingList = applicationFactory.userData.following;
                $scope.followingTable.reload();
            } else {
                $scope.onLoadingUser = true;
                userSrv.searchUser({ nameToSearch: $scope.followingFilter.name }).then(function (dt) {
                    $scope.onLoadingUser = false;
                    followingList = dt;
                    $scope.followingTable.reload();
                }, function () {
                    // Show error message
                    myAlert.pop('e', 'Error!', 'There was an error occurred while searching users');
                });
            }

        });

        // watch user typing name
        $scope.$watch("followingFilter", function () {
            $scope.$emit("reloadFollowingTable");
        }, true);

        // check following status
        $scope.isFollowed = function (name) {
            for (var i = 0; i < applicationFactory.userData.following.length; i++) {
                if (name == applicationFactory.userData.following[i].userName) {
                    return true;
                }
            }
            return false;
        };

        // view user info
        $scope.viewUserInfo = function (user) {
            $rootScope.$broadcast("openPopupViewUserInfo", { userName: user.userName, currentUserList: followingList });
        };

    }])

    // Password page
    .controller('UserPasswordCtrl', ['$scope', 'UserSrv', 'ApplicationFactory', 'WorkContextSrv','myAlert', function ($scope, userSrv, applicationFactory, workContextSrv,myAlert) {
        var currentUser = workContextSrv.getCurrentUser();                                      // Get current user object

        //create passwordmodel
        $scope.passwordModel = { oldPassword: '', newPassword: '', confirmNewPassword: '' };    // Password model

        $scope.savePasswordSuccessful = false;                                                  // Save password successful state
        $scope.savePasswordShowMessage = false;                                                 // Show message state
        $scope.savePasswordMessage = '';                                                        // Message

        // fn save password successful callback
        function savePasswordSuccessful(data) {
            if (data === '') {
                $scope.savePasswordSuccessful = true;
                $scope.savePasswordShowMessage = true;
                $scope.savePasswordMessage = 'Your password has been updated successful!';
                //reset form
                $scope.userpasswordform.$setPristine();
                //reset model
                $scope.passwordModel = angular.copy({ oldPassword: '', newPassword: '', confirmNewPassword: '' });
            }

            //hide indicator
            $scope.onSubmitting = false;
        }

        // fn save password error callback
        function savePasswordError(data) {
            $scope.savePasswordMessage = data;
            $scope.savePasswordSuccessful = false;
            $scope.savePasswordShowMessage = true;
            //hide indicator
            $scope.onSubmitting = false;
        }

        //save  password
        $scope.savePassword = function (passwordModel) {
            $scope.savePasswordShowMessage = false;
            if (passwordModel.newPassword === passwordModel.confirmNewPassword && passwordModel.newPassword.length > 1 && passwordModel.confirmNewPassword.length > 1 && passwordModel.oldPassword.length > 1) {
                //show indicator
                $scope.onSubmitting = true;

                userSrv.updateUserPassword({ name: currentUser.userName, oldp: passwordModel.oldPassword, newp: passwordModel.newPassword }).then(savePasswordSuccessful, savePasswordError);
            }
        };
    }])