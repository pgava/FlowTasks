/// <reference path="../scripts/jasmine.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/jquery-2.1.0.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-mocks.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-resource.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-ui-router.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-sanitize.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/ui-utils.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/ng-infinite-scroll.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/ng-table.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-webstorage.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-touch.min.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/ng-google-chart.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/factories/AlertFactory.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/global.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Content/themes/smart/js/datetime/moment.min.js" />

/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/flowTasksApp.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/UserController.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/directives/UserDirective.js" />

describe('User-->', function () {
    var workContextMock,
        userMock,
        loggingMock,
        applicationMock,
        taskFactoryMock,
        topicFactoryMock,
        $scope,
        ctrl,
        $timeout,
        rootScope,
        element,
        httpBackend;

    beforeEach(function () {

        module('flowTasksApp', function ($provide) {
            workContextMock = jasmine.createSpyObj('WorkContextSrv', ['isAuthenticated', 'getCurrentUser', 'setCurrentUser']);
            userMock = jasmine.createSpyObj('UserSrv', ['getUserByName', 'authorize', 'isAuthenticated', 'getUserLocalData', 'updateUserInfo', 'updateUserPassword', 'searchUser']);
            applicationMock = jasmine.createSpyObj('ApplicationFactory', ['updateUserData', 'abortPendingRequests']);
            topicFactoryMock = jasmine.createSpyObj('TopicFactory', ['loadTopicCounter']);
            taskFactoryMock = jasmine.createSpyObj('TaskFactory', ['loadTaskCounter']);
            loggingMock = jasmine.createSpyObj('LoggingSrv', ['log']);

            workContextMock.isAuthenticated.and.callFake(function () {
                return true;
            });
            workContextMock.getCurrentUser.and.callFake(function () {
                return { userName: 'TestUser' };
            });
            applicationMock.preLoading = {
                show: function() {}
            };
            userMock.isAuthenticated.and.callFake(function() {
                return true;
            });
            userMock.getUserLocalData.and.callFake(function() {
                return { userName: 'TestUser' };
            });
                
            // provide the mock!
            $provide.value('WorkContextSrv', workContextMock);
            $provide.value('UserSrv', userMock);
            $provide.value('ApplicationFactory', applicationMock);
            $provide.value('TaskFactory', taskFactoryMock);
            $provide.value('TopicFactory', topicFactoryMock);
            $provide.value('LoggingSrv', loggingMock);
        });

    });

    describe("controller->", function () {

        beforeEach(function () {

            inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

                httpBackend = $httpBackend;
                $scope = $rootScope.$new();

                rootScope = $rootScope;
                spyOn($rootScope, '$broadcast').and.callThrough();

                userMock.getUserByName.and.returnValue($q.when({}));

                $timeout = _$timeout_;

                ctrl = $controller('UserCtrl', {
                    $scope: $scope
                });

                httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
                $scope.$apply();
            });

        });

        it('should start with user initialzed', function () {
            expect($scope.currentTab).toEqual("info");
        });

        it('should change tab', function () {

            $scope.currentTab = "info";
            $scope.changeTab("task");

            expect($scope.currentTab).toEqual("task");
        });

        it('should check if is tab', function () {

            var res = $scope.isTab("info");

            expect(res).toEqual(true);
        });

    });

    describe("profile controller->", function () {

        beforeEach(function () {

            inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

                httpBackend = $httpBackend;
                $scope = $rootScope.$new();

                rootScope = $rootScope;
                spyOn($rootScope, '$broadcast').and.callThrough();

                userMock.getUserByName.and.returnValue($q.when({}));

                $timeout = _$timeout_;

                ctrl = $controller('UserProfileCtrl', {
                    $scope: $scope
                });

                httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
                $scope.$apply();
            });

        });

        it('should start with user profile initialzed', function () {
            expect($scope.currentUser).toBeDefined();
        });

        it('should save info', inject(function ($q) {

            userMock.updateUserInfo.and.returnValue($q.when({}));

            $scope.saveInfo({UserName:"TestUser"});
            $scope.$apply();

            expect($scope.saveInfoSuccessful).toEqual(true);
            expect($scope.saveInfoMessage).toEqual('Your info has been saved successful!');
            expect(workContextMock.setCurrentUser).toHaveBeenCalled();
            expect(applicationMock.updateUserData).toHaveBeenCalled();
        }));

    });

    describe("following controller->", function () {

        beforeEach(function () {

            inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

                httpBackend = $httpBackend;
                $scope = $rootScope.$new();

                rootScope = $rootScope;
                spyOn($rootScope, '$broadcast').and.callThrough();

                userMock.getUserByName.and.returnValue($q.when({}));

                applicationMock.userData = {};
                applicationMock.userData.following = [];

                $timeout = _$timeout_;

                ctrl = $controller('UserFollowingCtrl', {
                    $scope: $scope
                });

                httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
                $scope.$apply();
            });

        });

        it('should start with user following initialzed', function () {
            expect($scope.onLoadingUser).toEqual(false);
        });

        it('should reload following table', inject(function ($q) {

            userMock.searchUser.and.returnValue($q.when([{user: "aa"}]));

            $scope.followingFilter = { name: "aa" };
            $scope.$apply();

            var res = $scope.followingTable.total();
            expect(res).toEqual(1);
        }));
    });

    describe("password controller->", function () {

        beforeEach(function () {

            inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

                httpBackend = $httpBackend;
                $scope = $rootScope.$new();

                rootScope = $rootScope;
                spyOn($rootScope, '$broadcast').and.callThrough();

                userMock.getUserByName.and.returnValue($q.when({}));

                applicationMock.userData = {};
                applicationMock.userData.following = [];

                $timeout = _$timeout_;

                ctrl = $controller('UserPasswordCtrl', {
                    $scope: $scope
                });

                httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
                $scope.$apply();
            });

        });

        it('should start with user password initialzed', function () {
            expect($scope.passwordModel).toBeDefined();
        });

        it('should save password', inject(function ($q) {

            userMock.updateUserPassword.and.returnValue($q.when(''));
            $scope.userpasswordform = {};
            $scope.userpasswordform.$setPristine = function() {};

            $scope.savePassword({ newPassword: "newPassword", confirmNewPassword: "newPassword", oldPassword: "oldPassword" });
            $scope.$apply();

            expect($scope.savePasswordSuccessful).toEqual(true);
            expect($scope.savePasswordMessage).toEqual('Your password has been updated successful!');

        }));
    });

});