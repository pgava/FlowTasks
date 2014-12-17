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
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/TopicController.js" />
/// <reference path="../../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/services/TopicService.js" />

describe('Topic-->', function () {
    var topicSrvMock,
        workContextMock,
        userMock,
        loggingMock,
        applicationMock,
        topicMock,
        taskMock,
        httpBackend;

    var $scope, ctrl, $timeout, rootScope;

    beforeEach(function () {

        // load the module.
        module('flowTasksApp', function ($provide) {
            // Create a "spy object" for our services.
            // This will isolate the controller we're testing from
            // any other code.
            // we'll set up the returns for this later 
            workContextMock = jasmine.createSpyObj('WorkContextSrv', ['isAuthenticated', 'getCurrentUser', 'setCurrentUser']);
            userMock = jasmine.createSpyObj('UserSrv', ['getUserByName', 'authorize', 'isAuthenticated', 'getUserLocalData']);
            applicationMock = jasmine.createSpyObj('ApplicationFactory', ['updateUserData', 'abortPendingRequests']);
            topicMock = jasmine.createSpyObj('TopicFactory', ['loadTopicCounter', 'onReloadInvoke', 'setTopicCounter']);
            taskMock = jasmine.createSpyObj('TaskFactory', ['loadTaskCounter', 'loadUserStat']);
            topicSrvMock = jasmine.createSpyObj('TopicSrv', ['getTopics', 'getTopicReplies', 'searchTopics']);
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
            $provide.value('TopicFactory', topicMock);
            $provide.value('TaskFactory', taskMock);
            $provide.value('LoggingSrv', loggingMock);
        });

        // INJECT! This part is critical
        // $rootScope - injected to create a new $scope instance.
        // $controller - injected to create an instance of our controller.
        // $q - injected so we can create promises for our mocks.
        // _$timeout_ - injected to we can flush unresolved promises.
        inject(function ($httpBackend, $rootScope, $controller, $q, _$timeout_) {

            httpBackend = $httpBackend;
            $scope = $rootScope.$new();

            // Testing $broadcast
            rootScope = $rootScope;
            spyOn($rootScope, '$broadcast').and.callThrough();

            // set up the returns for our someServiceMock
            // $q.when('weee') creates a resolved promise to "weee".
            // this is important since our service is async and returns
            // a promise.
            topicMock.onReloadInvoke.and.returnValue($q.when(''));
            userMock.getUserByName.and.returnValue($q.when({}));
            taskMock.loadUserStat.and.returnValue($q.when(''));

            // assign $timeout to a scoped variable so we can use 
            // $timeout.flush() later. Notice the _underscore_ trick
            // so we can keep our names clean in the tests.
            $timeout = _$timeout_;

            // now run that scope through the controller function,
            // injecting any services or other injectables we need.
            // **NOTE**: this is the only time the controller function
            // will be run, so anything that occurs inside of that
            // will already be done before the first spec.
            ctrl = $controller('TopicCtrl', {
                $scope: $scope,
                TopicFactory: topicMock,
                TopicSrv: topicSrvMock
            });

            // Finish to initialize
            topicSrvMock.getTopics.and.returnValue($q.when([]));
            topicSrvMock.topics = {};
            topicSrvMock.topics.subjects = [];

            httpBackend.expectGET('scripts/flowTasksApp/views/Home/main.html').respond({});
            $scope.$apply();
        });

    });

    describe("Topic controller->", function () {

        /* The simplest of the simple.
         * here we're going to test that some things were 
         * populated when the controller function whas evaluated. */
        it('should start with topic initialzed', function () {
            expect($scope.topic.topicList.onLoading).toEqual(false);
        });

        /* Testing a $watch()
         * The important thing here is to call $apply() 
         * and THEN test the value it's supposed to update. */
        it('should update topic list with no result when query changed', inject(function ($q) {

            // Search result
            topicSrvMock.searchTopics.and.returnValue($q.when([]));
            topicSrvMock.topics = {};
            topicSrvMock.topics.subjects = [];
            topicSrvMock.topics.topicList = {};

            //spyOn($scope.topic, "loadMoreTopics");

            //change query
            $scope.topic.topicSearch.query = 'something to search';
            $scope.topic.topicList.firstLoad = false;

            //$apply the change to trigger the $watch.
            $scope.$apply();

            //assert
            //expect($scope.topic.loadMoreTopics).not.toHaveBeenCalled();
            expect($scope.topic.topicSearch.onSearching).toEqual(false);
            expect($scope.topic.topicSearch.searchResult).toEqual([]);
            expect($scope.topic.topicSearch.showMessage).toEqual(true);
        }));

        it('should update topic list with the result when loading topics', inject(function ($q) {

            // Search result
            topicSrvMock.getTopics.and.returnValue($q.when([{desc:'a'},{desc:'b'}]));
            topicSrvMock.topics = {};
            topicSrvMock.topics.subjects = [{ desc: 'a' }, { desc: 'b' }];
            topicSrvMock.topics.topicList = {};

            $scope.topic.loadMoreTopics(true);
            $scope.$apply();

            //assert
            expect($scope.topic.topicList.stopLoading).toEqual(true);
            expect($scope.topic.topicList.results.length).toEqual(2);
            expect($scope.topic.topicList.results[0].desc).toEqual('a');
            expect($scope.topic.topicList.results[1].desc).toEqual('b');
        }));

        it('should call the event to load the topics', inject(function ($q) {

            rootScope.$broadcast('TOPICS:RELOAD', '');
            expect(rootScope.$broadcast).toHaveBeenCalled();
        }));
    });

});