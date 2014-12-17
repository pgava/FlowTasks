flowTasksApp
    .factory("ApplicationFactory", ['$state', '$rootScope', '$timeout', function ($state, $rootScope, $timeout) {
        var applicationData = {
            userData: {
                userName: '',
                photoPath: '',
                firstName: '',
                lastName: '',
                pages: [],
                following: [],
                follower: [],
                followerCount: 0,
                followingCount: 0,
                email: ''
            },
            clearUserData: function () {
                this.updateUserData({
                    userName: '',
                    photoPath: '',
                    firstName: '',
                    lastName: '',
                    pages: [],
                    following: [],
                    follower: [],
                    followerCount: 0,
                    followingCount: 0,
                    email: ''
                });
            },
            updateUserData: function (ud) {
                if (angular.isDefined(ud.userName))
                    this.userData.userName = ud.userName;

                if (angular.isDefined(ud.photoPath))
                    this.userData.photoPath = ud.photoPath;

                if (angular.isDefined(ud.firstName))
                    this.userData.firstName = ud.firstName;

                if (angular.isDefined(ud.lastName))
                    this.userData.lastName = ud.lastName;

                if (angular.isDefined(ud.pages))
                    this.userData.pages = ud.pages;

                if (angular.isDefined(ud.following))
                    this.userData.following = ud.following;

                if (angular.isDefined(ud.follower))
                    this.userData.follower = ud.follower;

                if (angular.isDefined(ud.followerCount))
                    this.userData.followerCount = ud.followerCount;

                if (angular.isDefined(ud.followingCount))
                    this.userData.followingCount = ud.followingCount;
                    
                if (angular.isDefined(ud.email))
                    this.userData.email = ud.email;
            },
            app: {},
            page: {
                current: '',
                title: 'Flow Tasks - Home',
            },
            preLoading: {
                show: function (isShow) {
                    $rootScope.$broadcast('SHOW-HIDE-MENU', { isShow: isShow });
                }
            },
            showHiddenMenu: function () {
                // hide menu
                $rootScope.$broadcast('SHOW-HIDDEN-MENU');
            },
            abortPendingRequests: function () {
                //abort all previous request
                $rootScope.$broadcast("abortPendingRequest");
            }
        }

        return applicationData;
    }])