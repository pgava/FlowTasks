flowTasksApp
    .service('DashboardSrv', ['$http', 'helperSrv', 'globalConfig', '$q',
        function ($http, helperSrv, globalConfig, $q) {
            // Get workflows
            this.getWorkflows = function (filter, pageIndex) {
                var woid = (filter.woid !== '' && /\w{8}\-\w{4}\-\w{4}\-\w{4}\-\w{12}/g.test(filter.woid)) ? filter.woid : '';
                //var start = (filter.start !== '' && /\d{1,2}\/\d{1,2}\/\d{4}/g.test(filter.start)) ? filter.start : '';
                //var end = (filter.end !== '' && /\d{1,2}\/\d{1,2}\/\d{4}/g.test(filter.end)) ? filter.end : '';
                var start = filter.start;
                var end = filter.end;
                if (end !== '') {
                    //Start Increase 1 day in end date
                    //date string to date object
                    var endDateObject = helperSrv.convertFormatedStringToDateObject(end);
                    //Add a day
                    endDateObject = moment(endDateObject).add('d', 1);
                    //date object to date string
                    end = helperSrv.convertDateObjectToServerDate(endDateObject);
                    //End Increase 1 day in end date
                }
                if (start !== '') {
                    //convert formated date string to date object
                    var startDateString = helperSrv.convertFormatedStringToDateObject(start);
                    //convert date object to server date
                    start = helperSrv.convertDateObjectToServerDate(startDateString);
                }
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/dashboardworkflows', params: { woid: woid, wcode: filter.wcode, isActive: filter.isActive, start: start, end: end, pageIndex: pageIndex, pageSize: filter.pageSize }, cache: false })
                    .success(function (data) {
                        deferred.resolve(data);
                    }).error(function (err) {
                        deferred.reject(err);
                    });
                // return promise
                return deferred.promise;
            };

            // Do workflow
            this.doWorkflow = function (action, wfids) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'PATCH', url: 'api/dashboardworkflows', params: { op: action }, data: wfids, cache: false })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

            // Get workflow details
            this.getWorkflowDetail = function (woid) {
                // Make promise
                var deferred = $q.defer();
                // Do request
                $http({ method: 'GET', url: 'api/dashboarddetails/' + woid, cache: false })
                .success(function (data) {
                    deferred.resolve(data);
                }).error(function (err) {
                    deferred.reject(err);
                });
                // return promise
                return deferred.promise;
            };

        }
    ]);