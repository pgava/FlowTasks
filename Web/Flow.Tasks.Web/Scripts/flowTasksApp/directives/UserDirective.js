flowTasksApp
    .directive('userInfo', function () {
        return {
            restrict: "E",
            replace: true,
            controller: ['$scope', '$rootScope', 'UserSrv', 'WorkContextSrv', 'ApplicationFactory', 'myAlert', function ($scope, $rootScope, userSrv, workContextSrv, applicationFactory, myAlert) {

                var currentUserList = [];
                // open popup to view user info
                $rootScope.$on("openPopupViewUserInfo", function (e, params) {
                    currentUserList = [];
                    var userName = params.userName;                                 // username parameter
                    currentUserList = params.currentUserList;                       // Users array
                    $scope.onLoadingUser = false;                                   // The value indicating whether on loading user
                    $scope.followText = "Follow";                                   // Follow text
                    $scope.followed = false;                                        // The value indicating whether followed

                    if (!$scope.onLoadingUser) {
                        $scope.onLoadingUser = true;

                        // Do request get user info by username
                        userSrv.getUserByName(userName, true).then(function (dt) {
                            if (dt !== 'true') {
                                // show modal
                                jQuery('#viewuserinfo').modal('show');

                                //convert birtday format
                                var birtdayFormated = '';
                                $scope.userInfo = dt;
                                if (dt.birthday != null) {
                                    // parse birthday format yyyy-mm-ddThh:MM:ss to mm/dd/yyyy
                                    birtdayFormated = dt.birthday.replace(/(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/g,
                                                    function ($0, $1, $2, $3, $4, $5, $6) {
                                                        return $2 + "/" + $3 + "/" + $1; //mm/dd/yyyy
                                                    }
                                                );
                                    $scope.userInfo.birthday = birtdayFormated;
                                }

                                $scope.display = dt.userName != applicationFactory.userData.userName;

                                // Change not followed status to Followed status on selected user in users list
                                for (var i = 0; i < applicationFactory.userData.following.length; i++) {
                                    if (dt.userName == applicationFactory.userData.following[i].userName) {
                                        $scope.followed = dt.userName == applicationFactory.userData.following[i].userName;
                                        $scope.followText = "UnFollow";
                                        break;
                                    }
                                }

                                $scope.onLoadingUser = false;
                            }
                        });

                    }
                });

                // fn Follow user
                $scope.follow = function (user) {
                    // show indicator
                    $scope.onSubmitting = true;

                    // unfollow
                    if ($scope.followed) {
                        userSrv.removeFollowing({ userName: applicationFactory.userData.userName, followerName: user.userName }).then(function (dt, st) {
                            $scope.followed = false;
                            $scope.followText = "Follow";
                            // Change followed status to not followed status on selected user in users list
                            for (var i = 0; i < applicationFactory.userData.following.length; i++) {
                                if (applicationFactory.userData.following[i].userName == user.userName) {
                                    //remove follower from list
                                    applicationFactory.userData.following.splice(i, 1);
                                    //reload Table
                                    $scope.$broadcast("reloadFollowingTable", currentUserList);
                                    break;
                                }
                            }
                            //console.log(dt);
                            //hide indicator
                            $scope.onSubmitting = false;
                        }, function (dt, st) {
                            //console.log(dt);
                            //hide indicator
                            $scope.onSubmitting = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while unfollowing user');
                        });
                    } else { //follow
                        userSrv.followUser({ userName: applicationFactory.userData.userName, followerName: user.userName }).then(function (dt, st) {
                            $scope.followed = true;
                            $scope.followText = "UnFollow";

                            //add follower to following list
                            applicationFactory.userData.following.push(user);

                            //reload Table
                            $scope.$broadcast("reloadFollowingTable", currentUserList);
                            //console.log(dt);

                            //hide indicator
                            $scope.onSubmitting = false;
                        }, function (dt, st) {
                            //console.log(dt);
                            //hide indicator
                            $scope.onSubmitting = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while following user');
                        });
                    }

                };
            }],
            templateUrl: "scripts/flowTasksApp/views/User/info.html"
        };
    })

    // Following count
    .directive("followingCount", [function () {
        return {
            restrict: 'A',
            compile: function (elem) {
                elem.html("<h5>Following <span class='txt-color-orangeDark'>{{userData.followingCount}}</span></h5>");
            }
        };
    }])

    // Follower count
    .directive("followerCount", [function () {
        return {
            restrict: 'A',
            compile: function (elem) {
                elem.html("<h5>Followers <span class='txt-color-greenLight'>{{userData.followerCount}}</span></h5>");
            }
        };
    }])
;