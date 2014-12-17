flowTasksApp
    .factory("TopicFactory", ['$rootScope', 'WorkContextSrv', 'TopicSrv', '$q', 'LoggingSrv', '$state', function ($rootScope, workContextSrv, topicSrv, $q, loggingSrv, $state) {
        var onLoadingTopic = false;

        var topicData = {
            counter: 0,
            isBlink: false,
            reset: function () {
                this.counter = 0;
                this.isBlink = false;

                // Clear cached data
                topicSrv.removeCache();
            },
            setTopicCounter: function (number, isBlink) {
                this.counter = number;
                this.isBlink = isBlink;
            },
            loadTopicCounter: function (userName, setCounter) {
                var deferred = $q.defer();

                //Check authenticated
                if (workContextSrv.isAuthenticated()) {
                    // return if ajax is executing
                    if (onLoadingTopic) {
                        deferred.resolve();
                    } else {
                        // active topic loading state
                        onLoadingTopic = true;

                        //get topic count
                        topicSrv.getTopicNewCount(userName).then(function (data) {
                            // set topic counter
                            if (setCounter) {
                                topicData.setTopicCounter(data, data == 0 ? false : true);
                            }
                            // remove topic loading state
                            onLoadingTopic = false;

                            // Clear cached data
                            if (data != 0) {
                                topicSrv.removeCache();
                            }
                            deferred.resolve();
                        }, function (err) {
                            loggingSrv.log('loadTopicCounter error: ' + err.error);
                            //alert("Topic counter Error: " + err.st);

                            // clear topic counter
                            topicData.setTopicCounter(0, false);

                            // remove topic loading state
                            onLoadingTopic = false;

                            // Clear cached data
                            topicSrv.removeCache();

                            // redirect to sign in page
                            $state.go("signin");

                            deferred.reject();
                        });
                    }
                } else {
                    deferred.reject();
                }

                return deferred.promise;
            },
            reloadTopics: function () {
                // fire event
                $rootScope.$broadcast("TOPICS:RELOAD");
            },
            replyUrl: function (topicId, from, to) {
                return 'api/topics?tid=' + topicId + '&from=' + from + '&to=' + to;
            },
            createTopicUrl: function (from, to) {
                var url = 'api/topics';
                if (from || to) {
                    url += "?";
                    if (from) {
                        url += +"from=" + from + "&";
                    }
                    if (to) {
                        url += +"to=" + to + "&";
                    }
                }
                return url.replace(/&$/, '');           // trim '&' character
            }
        }

        return topicData;
    }])