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
        $timeout,
        rootScope,
        element,
        httpBackend,
        $compile;

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

    beforeEach(module('flowTasksDirectives'));

    describe("User directive following count->", function () {

        beforeEach(function () {

            inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_, $compile) {

                httpBackend = $httpBackend;
                $scope = $rootScope.$new();

                rootScope = $rootScope;
                spyOn($rootScope, '$broadcast').and.callThrough();

                userMock.getUserByName.and.returnValue($q.when({}));

                applicationMock.userData = {};
                applicationMock.userData.following = [];

                $timeout = _$timeout_;
                
                element = '<li class="sparks-info" data-following-count></li>';

                $scope.userData = {};
                $scope.userData.followingCount = 5;

                element = $compile(element)($scope);
                $scope.$digest();
            });

        });

        it('should set the following counter', function () {
            expect(element[0].innerText).toBeDefined("Following 5");
        });
        
    });

    describe("User directive info user->", function () {

        beforeEach(function () {

            inject(function ($rootScope, _$compile_) {

                $scope = $rootScope.$new();
                $compile = _$compile_;

                rootScope = $rootScope;
            });

        });

        beforeEach(inject(function (_$compile_, $rootScope) {
            $scope = $rootScope.$new();
            $compile = _$compile_;
        }));

        it('should set the user info', function () {

            $scope.userInfo = {};
            $scope.userInfo.firstName = 'Test';
            $scope.userInfo.lastName = 'User';

            var elUserInfo = '<user-info></user-info>';
            elUserInfo = $compile(elUserInfo)($scope);
            $scope.$digest();

            var h3 = elUserInfo.find('h3');

            // Create an instance of the directive
            //var element1 = angular.element('<user-info></user-info>');
            //$compile(element1)($scope); // Compile the directive
            //$scope.$digest(); // Update the HTML

            expect(elUserInfo.html()).toContain('Test User');
        });

    });
});