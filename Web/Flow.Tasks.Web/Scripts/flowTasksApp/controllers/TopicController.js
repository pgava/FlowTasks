flowTasksApp
    .controller('TopicCtrl', [
        "$scope", 'TopicSrv', '$timeout', '$q', 'WorkContextSrv', '$rootScope', '$document', 'TopicFactory', 'ApplicationFactory', 'myAlert', function ($scope, topicSrv, $timeout, $q, workContextSrv, $rootScope, $document, topicFactory, applicationFactory, myAlert) {
            var currentUser = workContextSrv.getCurrentUser();                                // Current user object
            var username = currentUser.userName;                                              // Get username
            var defaultPageSize = 10;                                                         // Default page size
            var defaultPageIndex = 0;                                                         // Default page index
            var pageSize = defaultPageSize;
            var pageIndex = defaultPageIndex;
            var loadtm = null;                                                                // Load data Timeout object

            $scope.topic = {
                topicList: {
                    onLoading: false,                                                         // On Topic Loading State
                    stopLoading: false,                                                       // Stop Topic Loading State (If true that mean no more topics to load)
                    noResult: false,                                                          // No result state
                    results: [],                                                              // Topics list array
                    firstLoad: true,                                                          // First load state (Prevent 'Watch' excute the callback in first load page)
                    onLoadingSingleTopic: false                                               // On loading single topic state (When click topic from search result)
                },
                topicSearch: {
                    searchResult: [],                                                         // Topics search result array
                    showSearch: false,                                                        // Show search result box state
                    onSearching: false,                                                       // On searhing state
                    query: '',                                                                // Query
                    status: '',                                                               // Status
                    showMessage: false                                                        // Show message when searching no result
                },
                currentDate: [],                                                              // Dates Array
                // fn Show date 
                showDate: function (date) {
                    if ($scope.topic.currentDate.indexOf(date) !== -1) {
                        return false;
                    }

                    $scope.topic.currentDate.push(date);
                    return true;
                },
                chooseSearchItem: function (sitem) {                                                    // fn topic search result click
                    // Turn topic searching state on
                    $scope.topic.topicSearch.onSearching = true;
                    // Clear Topics list
                    $scope.topic.topicList.results = [];
                    // Check if request get topic details was done
                    if (!$scope.topic.topicList.onLoadingSingleTopic) {
                        // Turn single topic loading state on
                        $scope.topic.topicList.onLoadingSingleTopic = true;
                        // Do request get topic by id
                        topicSrv.getTopicById(username, sitem.id)
                            .then(function (dt) {
                                // success goes here
                                if (dt.subjects.length) {
                                    $scope.topic.topicList.noResult = false;
                                    // Add Topic to Topics List
                                    $scope.topic.topicList.results.push(dt.subjects[0]);
                                }
                                // Turn states off
                                $scope.topic.topicSearch.showSearch = false;
                                $scope.topic.topicSearch.onSearching = false;
                                $scope.topic.topicList.onLoadingSingleTopic = false;
                                $scope.topic.topicList.stopLoading = true;
                            }, function (err) {
                                // error goes here
                                // Turn states off
                                $scope.topic.topicSearch.showSearch = false;
                                $scope.topic.topicSearch.onSearching = false;
                                $scope.topic.topicList.onLoadingSingleTopic = false;
                                $scope.topic.topicList.stopLoading = true;
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading topic');
                            });
                    }
                },
                // Create new topic view model
                newTopicModel: {},
                // fn add topic to topics list
                addNewTopicToList: function (newTopicModel) {
                    var resp = newTopicModel;
                    if (resp.result) {
                        var newTopic = resp.message;
                        var replies = resp.replies;

                        var ix = $scope.topic.currentDate.indexOf(resp.message.whenDay);
                        if (ix !== -1) {
                            if ($scope.topic.topicList.results.length > 0) {
                                $scope.topic.topicList.results[0].showDateDivider = false;
                                $scope.topic.currentDate.splice(ix, 1);
                            }
                        }

                        $scope.topic.topicList.results.unshift({ "status": "New", "title": null, "topicId": resp.topicId, "message": newTopic, "replies": replies });

                        //hide 'no topic' message when create new topic
                        $scope.topic.topicList.noResult = false;
                    } else {
                        //alert(resp.errorMessage);
                    }
                },
                // fn Load topics
                loadMoreTopics: loadMoreTopics
            }

            // watch query change
            var topicSearchListener = $scope.$watch('topic.topicSearch.query', function () {
                //abort all previous request
                applicationFactory.abortPendingRequests();

                // set default page size
                pageSize = defaultPageSize;

                // set defailt page index
                pageIndex = defaultPageIndex;
                // first load state
                if (!$scope.topic.topicList.firstLoad) {
                    // if query is not empty
                    if ($scope.topic.topicSearch.query === '') {
                        // disable stop loading state
                        $scope.topic.topicList.stopLoading = false;

                        // hide no result message
                        $scope.topic.topicList.noResult = false;

                        // disable on searching state
                        $scope.topic.topicSearch.onSearching = false;

                        // hide search result
                        $scope.topic.topicSearch.showSearch = false;
                        // load topics
                        loadMoreTopics(true);
                    } else {
                        // search topics
                        searchTopic();
                    }
                }
            });

            // Watch reload topics event
            var reloadListener = $rootScope.$on('TOPICS:RELOAD', function () {
                $scope.topic.topicList.stopLoading = false;
                $scope.topic.topicList.noResult = false;
                loadMoreTopics(true);
            });

            // fn search topic
            function searchTopic() {
                if ($scope.topic.topicSearch.query.length < 3) return;

                $scope.topic.topicList.stopLoading = false;
                $scope.topic.topicList.onLoading = false;

                $scope.topic.topicSearch.onSearching = true;
                $scope.topic.topicSearch.showSearch = true;
                $scope.topic.topicSearch.searchResult = [];
                $scope.topic.topicSearch.showMessage = false;
                topicSrv.searchTopics(username, $scope.topic.topicSearch.query)
                    .then(function (data) {
                        $scope.topic.topicSearch.searchResult = data;
                        $scope.topic.topicSearch.showMessage = !data.length;
                        $scope.topic.topicSearch.onSearching = false;
                        loadtm = null;
                    }, function (err, st) {
                        //error goes here
                        $scope.topic.topicSearch.onSearching = false;
                        if (angular.isDefined(st)) {        // except abort error
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while searching topic');
                        }
                    });
            }

            // fn Load topics
            function loadMoreTopics(clear) {
                if (clear) {
                    $scope.topic.topicList.results = [];
                    pageSize = defaultPageSize;
                    pageIndex = defaultPageIndex;
                    $scope.topic.topicList.onLoading = false;
                    $scope.topic.topicList.stopLoading = false;

                    $scope.topic.currentDate = [];
                }

                if (!$scope.topic.topicList.onLoading && !$scope.topic.topicList.stopLoading) {
                    //abort all previous request
                    applicationFactory.abortPendingRequests();

                    $scope.topic.topicList.onLoading = true;
                    $scope.topic.topicSearch.query = '';
                    topicSrv.getTopics(username, '', $scope.topic.topicSearch.status, pageSize, pageIndex++, false)
                        .then(function () {
                            //if there is some to load more
                            if (!$scope.topic.topicList.stopLoading) {

                                Array.prototype.push.apply($scope.topic.topicList.results, topicSrv.topics.subjects);

                                if ($scope.topic.topicList.results.length <= 0) {
                                    $scope.topic.topicList.noResult = true;
                                }

                                //if the result lesser than pageSize then stop loading in future
                                if (topicSrv.topics.subjects.length < pageSize) {
                                    $scope.topic.topicList.stopLoading = true;
                                }
                            }

                            if (pageIndex === 0) {
                                //clear topic counter when load topics
                                topicFactory.setTopicCounter(0, false);
                            }

                            // disable first load state
                            $scope.topic.topicList.firstLoad = false;
                            // disable on loading state
                            $scope.topic.topicList.onLoading = false;
                            // clear pending request
                            loadtm = null;

                            // clear topic counter blink and reset counter
                            topicFactory.setTopicCounter(0, false);
                        },
                        function (resp) {
                            if (resp.st != 401 && angular.isDefined(resp.st)) {
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading topics');
                            }

                            $scope.topic.topicList.onLoading = false;
                        })
                        .then(function () {
                            applicationFactory.preLoading.show(false);
                        });

                }
            }

            $scope.$on("$destroy", function () {
                // remove listener
                reloadListener();
                topicSearchListener();
            });
        }
    ]);