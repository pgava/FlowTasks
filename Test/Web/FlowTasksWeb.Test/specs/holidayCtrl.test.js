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
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/HolidayController.js" />

describe('Holiday-->', function () {
    var holidaySrvMock,
        workContextMock,
        userMock,
        loggingMock,
        applicationMock,
        taskFactoryMock,
        topicFactoryMock,
        holidayFactoryMock,
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
            holidayFactoryMock = jasmine.createSpyObj('HolidayFactory', ['getDateStatus']);
            holidaySrvMock = jasmine.createSpyObj('HolidaySrv', ['getUserHolidays', 'applyHolidays', 'startHolidayWorkflow']);
            loggingMock = jasmine.createSpyObj('LoggingSrv', ['log']);

            workContextMock.isAuthenticated.and.callFake(function () {
                return true;
            });
            workContextMock.getCurrentUser.and.callFake(function () {
                return { userName: 'TestUser', following: [] };
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
            $provide.value('HolidayFactory', holidayFactoryMock);
            $provide.value('HolidaySrv', holidaySrvMock);
            $provide.value('LoggingSrv', loggingMock);
        });

        inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {
            httpBackend = $httpBackend;

            $scope = $rootScope.$new();

            rootScope = $rootScope;
            spyOn($rootScope, '$broadcast').and.callThrough();

            userMock.getUserByName.and.returnValue($q.when({}));

            holidaySrvMock.getUserHolidays.and.returnValue($q.when([]));

            $timeout = _$timeout_;

            ctrl = $controller('HolidayCtrl', {
                $scope: $scope
            });

            httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
            $scope.$apply();
        });

    });

    describe("Holiday controller->", function () {

        it('should start with holiday initialzed', function () {

            expect($scope.currentTab).toEqual("mine");
        });

       it('should submit holiday', inject(function ($q) {

            holidaySrvMock.getUserHolidays.and.returnValue($q.when([{ holiday: ['2014/11/01'] }]));

            holidaySrvMock.applyHolidays.and.returnValue($q.when({ holidayId: 1 }));
            holidaySrvMock.startHolidayWorkflow.and.returnValue($q.when({}));

            var submit = { dates: [{ date: '2014/11/01' }], note:"aaa" };
            $scope.holiday.submit.selectedDates = submit.dates;
            $scope.submitHoliday(submit);
            $scope.$apply();

            expect($scope.holiday.submit.success).toEqual(true);
            expect($scope.holiday.submit.successMessage).toEqual('Your Holidays was submitted');
            expect($scope.holiday.submit.markedDates.length).toEqual(1);
            expect($scope.holiday.submit.markedDates[0].date).toEqual('2014/11/01');
            expect($scope.holiday.submit.selectedDates.length).toEqual(0);
            expect($scope.holiday.submit.note).toEqual('');
        }));
        
        it('should view user holiday', function () {

            $scope.viewUserHoliday({ userName: "TestUser" });

            expect($scope.holiday.viewOther.selectedUser).toEqual("TestUser");
        });

    });

});