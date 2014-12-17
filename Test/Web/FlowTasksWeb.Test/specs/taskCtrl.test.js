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
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/TaskController.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/services/TaskService.js" />

describe('Task-->', function () {
    var taskSrvMock,
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
            taskFactoryMock = jasmine.createSpyObj('TaskFactory', ['loadTaskCounter', 'setTaskCounter']);
            taskSrvMock = jasmine.createSpyObj('TaskSrv', ['getTasksCount', 'getTasksByUser', 'getTaskByToid', 'getCommentsForTask', 'taskOperation']);
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
            $provide.value('TaskSrv', taskSrvMock);
            $provide.value('LoggingSrv', loggingMock);
        });

        inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {
            httpBackend = $httpBackend;
            $scope = $rootScope.$new();

            rootScope = $rootScope;
            spyOn($rootScope, '$broadcast').and.callThrough();

            userMock.getUserByName.and.returnValue($q.when({}));

            $timeout = _$timeout_;

            ctrl = $controller('TaskCtrl', {
                $scope: $scope,
                TaskFactory: taskFactoryMock,
                TaskSrv: taskSrvMock
            });

            httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
            $scope.$apply();
        });

    });


    describe("Task controller->", function () {

        it('should start with task initialzed', function () {
            expect($scope.taskList.onLoading).toEqual(false);
        });

        it('should load task list', inject(function ($q) {

            taskSrvMock.getTasksByUser.and.returnValue($q.when([{task:1}, {task:2}]));
            taskSrvMock.tasks = [{ task: 1 }, { task: 2 }];

            $scope.loadMoreTasks();
            $scope.$apply();

            expect($scope.taskList.onLoading).toEqual(false);
            expect($scope.taskList.results.length).toEqual(2);
            expect($scope.taskList.results[0].task).toEqual(1);
            expect($scope.taskList.results[1].task).toEqual(2);
            expect($scope.taskList.stopLoading).toEqual(true);

        }));

        it('should call the event to load the tasks', inject(function ($q) {

            taskSrvMock.getTasksByUser.and.returnValue($q.when([{ task: 1 }, { task: 2 }]));
            taskSrvMock.tasks = [{ task: 1 }, { task: 2 }];

            rootScope.$broadcast('TASKS:RELOAD', '');
            expect(rootScope.$broadcast).toHaveBeenCalled();
        }));

        it('should choose task', inject(function ($q) {

            var item = { task: 1 };
            $scope.chooseTaskItem(item);

            expect($scope.taskList.results.length).toEqual(1);
            expect($scope.taskList.results[0].task).toEqual(1);
            expect($scope.taskList.noResult).toEqual(false);
            expect($scope.taskSearch.showSearch).toEqual(false);
            expect($scope.taskSearch.onSearching).toEqual(false);
        }));

        it('should go back to list', inject(function ($q) {

            $scope.backToTasklist();

            expect($scope.taskDetail.enabled).toEqual(false);
        }));
    });

});