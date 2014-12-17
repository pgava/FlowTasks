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

/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/flowTasksApp.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/DashboardController.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/services/TaskService.js" />


describe('Dashboard-->', function () {
    var dashboardSrvMock,
        workContextMock,
        userMock,
        loggingMock,
        applicationMock,
        taskFactoryMock,
        topicFactoryMock,
        $scope,
        ctrl,
        $timeout,
        rootScope,
        httpBackend;

    beforeEach(function () {

        module('flowTasksApp', function ($provide) {
            workContextMock = jasmine.createSpyObj('WorkContextSrv', ['isAuthenticated', 'getCurrentUser', 'setCurrentUser']);
            userMock = jasmine.createSpyObj('UserSrv', ['getUserByName', 'authorize', 'isAuthenticated', 'getUserLocalData']);
            applicationMock = jasmine.createSpyObj('ApplicationFactory', ['updateUserData', 'abortPendingRequests']);
            topicFactoryMock = jasmine.createSpyObj('TopicFactory', ['loadTopicCounter']);
            taskFactoryMock = jasmine.createSpyObj('TaskFactory', ['loadTaskCounter']);
            dashboardSrvMock = jasmine.createSpyObj('DashboardSrv', ['doWorkflow', 'getWorkflowDetail', 'getWorkflows']);
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
                
            // provide the mocks!
            $provide.value('WorkContextSrv', workContextMock);
            $provide.value('UserSrv', userMock);
            $provide.value('ApplicationFactory', applicationMock);
            $provide.value('TaskFactory', taskFactoryMock);
            $provide.value('TopicFactory', topicFactoryMock);
            $provide.value('DashboardSrv', dashboardSrvMock);
            $provide.value('LoggingSrv', loggingMock);
        });

        inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

            httpBackend = $httpBackend;
            $scope = $rootScope.$new();

            rootScope = $rootScope;
            spyOn($rootScope, '$broadcast').and.callThrough();

            userMock.getUserByName.and.returnValue($q.when({}));

            dashboardSrvMock.getWorkflows.and.returnValue($q.when({ workflows: [{ parentid: 1 }, { parentid: 2 }], workflowCodes: [] }));

            $timeout = _$timeout_;

            ctrl = $controller('DashboardCtrl', {
                $scope: $scope,
                TaskFactory: taskFactoryMock,
                DashboardSrv: dashboardSrvMock
            });

            // this can be removed
            httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
            $scope.$apply();
        });

    });

    describe("Dashboard controller->", function () {

        it('should start with dashboard initialzed', function () {
            expect($scope.dashboard.firstLoad).toEqual(false);
        });

        it('should get workflow child', function () {
            var res = $scope.getWorkflowChild(1);
            expect(res.length).toEqual(1);
            expect(res[0].parentid).toEqual(1);
        });

        it('should expand workflow child', function () {

            $scope.expandChild({ wfid: 1 });

            expect($scope.dashboard.workflow.expandedWorkflows.length).toEqual(1);
            expect($scope.dashboard.workflow.expandedWorkflows[0]).toEqual(1);
        });

        it('should warn user to select workflow', function () {

            $scope.doAction("action");

            expect($scope.dashboard.workflow.message).toEqual('Please select at least 1 workflow');
        });

        it('should perform action to selected workflow', inject(function ($q) {

            dashboardSrvMock.doWorkflow.and.returnValue($q.when({}));
            $scope.dashboard.workflow.selectedWorkflow = [{ wfid: 1 }];
            applicationMock.userData = { userName: "TestUser" };

            $scope.doAction("action");
            $scope.$apply();

            expect($scope.dashboard.workflow.hasErrors).toEqual(false);
            expect($scope.dashboard.workflow.message).toEqual('action selected workflows successful');
        }));

        it('should give back errors to user', inject(function ($q) {

            dashboardSrvMock.doWorkflow.and.returnValue($q.when([{id:1, message:"error"}]));
            $scope.dashboard.workflow.selectedWorkflow = [{ wfid: 1 }];
            applicationMock.userData = { userName: "TestUser" };

            $scope.doAction("action");
            $scope.$apply();

            expect($scope.dashboard.workflow.hasErrors).toEqual(true);
            expect($scope.dashboard.workflow.messages.length).toEqual(1);
            expect($scope.dashboard.workflow.messages[0].message).toEqual("error");
        }));

        it('should view workflow details', inject(function ($q) {

            dashboardSrvMock.getWorkflowDetail.and.returnValue($q.when({}));

            $scope.viewWorkflowDetail({wfid:1});
            $scope.$apply();

            expect($scope.dashboard.workflow.onViewDetail).toEqual(true);
            expect($scope.dashboard.workflow.onLoading).toEqual(false);
            expect($scope.dashboard.workflow.detail.wfid).toEqual(1);
        }));
    });

});