flowTasksApp
    // Topic cache service
    .service('topicCacheSrv', ['$cacheFactory', function ($cacheFactory) {
        return $cacheFactory('topicsData');
    }
    ])

    // Topic reply cache service
    .service('replyCacheSrv', ['$cacheFactory', function ($cacheFactory) {
        return $cacheFactory('repliesData');
    }
    ])

    // Topic service
    .service('TopicSrv', ['$http', '$q', 'topicCacheSrv', 'replyCacheSrv', function ($http, $q, topicCacheSrv, replyCacheSrv) {
        var _topics = [];
        var _replies = [];

        _removeCache = function () {
            replyCacheSrv.removeAll();
            topicCacheSrv.removeAll();
        };

        // Topics list
        this.topics = _topics;

        // Replies list
        this.replies = _replies;

        // Remove cache method
        this.removeCache = _removeCache;

        // Get Topics
        this.getTopics = function (userName, query, status, pageSize, pageIndex, winthReplies) {
            // Make promise
            var deferred = $q.defer();
            // Get Cache data
            var t = topicCacheSrv.get('topic-' + pageIndex);

            // Get Cache data if not null
            if (typeof t !== 'undefined') {
                angular.copy(t, _topics);
                deferred.resolve();
            } else {
                // Do request
                $http({ method: 'GET', url: 'api/topics', params: { user: userName, query: query, status: status, pageSize: pageSize, pageIndex: pageIndex, winthReplies: winthReplies } })
                    .then(function (result) {
                        angular.copy(result.data, _topics);
                        // cache only the first 5 pages
                        if (pageIndex <= 5) {
                            topicCacheSrv.put('topic-' + pageIndex, result.data);
                        }
                        deferred.resolve();
                    },
                        function (error, st) {
                            deferred.reject({ error: error, st: st });
                        });
            }

            return deferred.promise;
        };

        // Get Topic Replies
        this.getTopicReplies = function (userName, toOid, showType) {
            // Make promise
            var deferred = $q.defer();
            // Get cache data
            var found = false;
            if (showType === 'Recent') {
                var r = replyCacheSrv.get('replies');
                if (typeof r !== 'undefined') {
                    angular.copy(r, _replies);
                    found = true;
                    deferred.resolve();
                }
            }

            if (!found) {
                // Do request
                $http({ method: 'GET', url: 'api/replies/' + toOid, params: { user: userName, showType: showType } })
                    .then(function (result) {
                        angular.copy(result.data, _replies);
                        if (showType === 'Recent') {
                            var info = replyCacheSrv.info();
                            if (info.size <= 5) {
                                replyCacheSrv.put('replies-' + toOid, result.data);
                            }
                        }
                        deferred.resolve();
                    },
                        function (error) {
                            deferred.reject();
                        });
            }
            // Return promise
            return deferred.promise;
        };

        // Search Topics
        this.searchTopics = function (userName, query, timeout) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({ method: 'GET', url: 'api/topics', params: { user: userName, searchFor: query }, cache: false, timeout: timeout })
            .success(function (data) {
                deferred.resolve(data);
            }).error(function (error, st) {
                deferred.reject({ error: error, st: st });
            });

            // Return promise
            return deferred.promise;
        };

        // Get topic by identifier
        this.getTopicById = function (userName, topicId) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({ method: 'GET', url: 'api/topics', params: { user: userName, topicId: topicId }, cache: false })
            .success(function (data) {
                deferred.resolve(data);
            }).error(function (err) {
                deferred.reject(err);
            });
            // return promise
            return deferred.promise;
        };

        // Get topic new count
        this.getTopicNewCount = function (userName) {
            // Make promise
            var deferred = $q.defer();
            // Do request
            $http({ method: 'GET', url: 'api/topics', params: { user: userName }, cache: false, headers: { 'Abortable': false } })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err, st) {
                    deferred.reject({ error: err, st: st });
                });
            // Return Promise
            return deferred.promise;
        };

        // Add Topic reply
        this.addReply = function (params, dt) {
            _removeCache();
            // Make promise
            var defered = $q.defer();
            // Do request
            $http({
                method: 'POST',
                url: 'api/topics',
                params: params,
                data: dt,
                cache: false,
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    if (typeof FormData != 'undefined') {
                        var fd = new FormData();
                        angular.forEach(data, function (value, key) {
                            if (key === 'filesList') {
                                angular.forEach(value, function (value1, key1) {
                                    fd.append(key1, value1);
                                });
                            }
                            fd.append(key, value);
                        });
                        return fd;
                    }
                }
            }).success(function (data) {
                defered.resolve(data);
            }).error(function (err) {
                defered.reject(err);
            });
            // Return Promise
            return defered.promise;
        };

        // Add new topic
        this.addTopic = function (params, dt) {
            _removeCache();
            // Make promise
            var defered = $q.defer();
            // Do request
            $http({
                method: 'POST',
                url: 'api/topics',
                params: params,
                data: dt,
                cache: false,
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    if (typeof FormData != 'undefined') {
                        var fd = new FormData();
                        angular.forEach(data, function (value, key) {
                            if (key === 'filesList') {
                                angular.forEach(value, function (value1, key1) {
                                    fd.append(key1, value1);
                                });
                            }
                            fd.append(key, value);
                        });
                        return fd;
                    }
                }
            }).success(function (data) {
                defered.resolve(data);
            }).error(function (err) {
                defered.reject(err);
            });
            // Return Promise
            return defered.promise;
        };

        // Download topic attachment
        this.downloadAttachment = function (oid) {
            // Make promise
            var defered = $q.defer();
            // Do request
            $http({ method: 'GET', url: 'api/docs/' + oid, params: {}, cache: false })
            .success(function (data) {
                defered.resolve(data);
            }).error(function (err) {
                defered.reject(err);
            });
            // Return Promise
            return defered.promise;
        };

        // Upload attachment
        this.uploadAttachment = function (from) {
            // Make promise
            var defered = $q.defer();
            // Do request
            $http({ method: 'POST', url: 'api/topics', params: { from: from }, cache: false })
                .success(function (data) {
                    defered.resolve(data);
                }).error(function (err) {
                    defered.reject(err);
                });
            // Return Promise
            return defered.promise;
        };

    }
    ]);