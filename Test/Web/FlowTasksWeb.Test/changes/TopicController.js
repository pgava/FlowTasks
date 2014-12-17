flowTasksApp
    .controller('TopicCtrl', [
        "$scope", 'TopicSrv', '$timeout', '$q', 'WorkContextSrv', '$rootScope', '$document', 'TopicFactory', 'ApplicationFactory', 'myAlert', function ($scope, topicSrv, $timeout, $q, workContextSrv, $rootScope, $document, topicFactory, applicationFactory, myAlert) {
            var currentUser = workContextSrv.getCurrentUser();
            var username = currentUser.userName;
            var defaultPageSize = 10;
            var defaultPageIndex = 0;
            var pageSize = defaultPageSize;
            var pageIndex = defaultPageIndex;
            var loadtm = null;

            $scope.myLoadMoreTopics = loadMoreTopics;

            $scope.topic = {
                topicList: {
                    onLoading: false,
                    stopLoading: false,
                    noResult: false,
                    results: [],
                    firstLoad: true,
                    onLoadingSingleTopic: false
                },
                topicSearch: {
                    searchResult: [],
                    showSearch: false,
                    onSearching: false,
                    query: '',
                    status: '',
                    showMessage: false
                },
                currentDate: [],
                showDate: function (date) {
                    if ($scope.topic.currentDate.indexOf(date) !== -1) {
                        return false;
                    }

                    $scope.topic.currentDate.push(date);
                    return true;
                },
                chooseSearchItem: function (sitem) {
                    $scope.topic.topicSearch.onSearching = true;

                    $scope.topic.topicList.results = [];

                    if (!$scope.topic.topicList.onLoadingSingleTopic) {

                        $scope.topic.topicList.onLoadingSingleTopic = true;
                        topicSrv.getTopicById(username, sitem.id)
                            .then(function (dt) {
                                // success goes here
                                if (dt.subjects.length) {
                                    $scope.topic.topicList.noResult = false;
                                    $scope.topic.topicList.results.push(dt.subjects[0]);
                                }
                                $scope.topic.topicSearch.showSearch = false;
                                $scope.topic.topicSearch.onSearching = false;
                                $scope.topic.topicList.onLoadingSingleTopic = false;
                                $scope.topic.topicList.stopLoading = true;
                            }, function (err) {
                                // error goes here
                                // remove states
                                $scope.topic.topicSearch.showSearch = false;
                                $scope.topic.topicSearch.onSearching = false;
                                $scope.topic.topicList.onLoadingSingleTopic = false;
                                $scope.topic.topicList.stopLoading = true;
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading topic');
                            });
                    }
                },
                newTopicModel: {},
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
                loadMoreTopics: loadMoreTopics
            }

            // watch query change
            $scope.$watch('topic.topicSearch.query', function () {
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
            $rootScope.$on('TOPICS:RELOAD', function () {
                $scope.topic.topicList.stopLoading = false;
                $scope.topic.topicList.noResult = false;
                loadMoreTopics(true);
            });

            // search topic function
            function searchTopic() {
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
                    }, function (err) {
                        //error goes here
                        $scope.topic.topicSearch.onSearching = false;
                        // Show error message
                        myAlert.pop('e', 'Error!', 'There was an error occurred while searching topic');
                    });
            }

            //load topics function
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
                        },
                        function (error) {
                            $scope.topic.topicList.onLoading = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while loading topics');
                        })
                        .then(function () {
                            applicationFactory.preLoading.show(false);
                        });

                }
            }
        }
    ]);