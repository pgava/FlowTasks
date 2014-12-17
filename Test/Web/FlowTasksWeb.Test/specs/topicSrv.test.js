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
    var topicSrv,
        httpBackend,
        workContextMock,
        userMock,
        loggingMock,
        applicationMock,
        topicMock,
        taskMock;

    beforeEach(function () {
        // load the module.
        module('flowTasksApp', function ($provide) {
            workContextMock = jasmine.createSpyObj('WorkContextSrv', ['isAuthenticated', 'getCurrentUser', 'setCurrentUser']);
            userMock = jasmine.createSpyObj('UserSrv', ['getUserByName', 'authorize', 'isAuthenticated', 'getUserLocalData']);
            applicationMock = jasmine.createSpyObj('ApplicationFactory', ['updateUserData', 'abortPendingRequests']);
            topicMock = jasmine.createSpyObj('TopicFactory', ['loadTopicCounter', 'onReloadInvoke']);
            taskMock = jasmine.createSpyObj('TaskFactory', ['loadTaskCounter', 'loadUserStat']);
            loggingMock = jasmine.createSpyObj('LoggingSrv', ['log']);

            workContextMock.isAuthenticated.and.callFake(function () {
                return true;
            });
            workContextMock.getCurrentUser.and.callFake(function () {
                return { userName: 'TestUser' };
            });
            applicationMock.preLoading = {
                show: function () { }
            };
            userMock.isAuthenticated.and.callFake(function () {
                return true;
            });
            userMock.getUserLocalData.and.callFake(function () {
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

        // get your service, also get $httpBackend
        // $httpBackend will be a mock, thanks to angular-mocks.js
        inject(function ($httpBackend, _TopicSrv_) {
            topicSrv = _TopicSrv_;
            httpBackend = $httpBackend;
        });

    });

    // make sure no expectations were missed in your tests.
    // (e.g. expectGET or expectPOST)
    afterEach(function () {
        httpBackend.verifyNoOutstandingExpectation();
        httpBackend.verifyNoOutstandingRequest();
    });

    describe("Topic service->", function () {

        it('should get the topics.', function () {
            // set up some data for the http call to return and test later.
            var returnData = [{
                Title: "title1",
                Description: "Desc1"
            },
                {
                    Title: "title2",
                    Description: "Desc2"
                }
            ];

            // expectGET to make sure this is called once.
            httpBackend.expectGET('api/topics').respond(returnData);

            // make the call.
            var returnedPromise = topicSrv.getTopics();

            // set up a handler for the response, that will put the result
            // into a variable in this scope for you to test.
            var result;
            returnedPromise.then(function () {
                result = topicSrv.topics;
            });

            // flush the backend to "execute" the request to do the expectedGET assertion.
            httpBackend.flush();

            // check the result. 
            // (after Angular 1.2.5: be sure to use `toEqual` and not `toBe`
            // as the object will be a copy and not the same instance.)
            expect(result).toEqual(returnData);
        });

    });
   
});