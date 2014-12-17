flowTasksApp
    .controller('HolidayCtrl', [
        "$scope", "WorkContextSrv", 'ApplicationFactory', '$timeout', '$rootScope', 'HolidaySrv', '$q', 'ngTableParams', '$filter', 'UserSrv', 'globalConfig', 'helperSrv', 'myAlert', 'HolidayFactory', function ($scope, workContextSrv, applicationFactory, $timeout, $rootScope, holidaySrv, $q, ngTableParams, $filter, userSrv, globalConfig, helperSrv, myAlert, holidayFactory) {
            var currentUser = workContextSrv.getCurrentUser();                      // Get current user object 
            var username = currentUser.userName;                                    // Get username
            var canceler;                                                           
            var currentOtherYearIndex = 1;                                          // Current year (Other)
            var numerOfDatePicker = [1, 3];                                         // Number of Calendars to be displayed
            var currentYearIndex = 1;                                               // Current Year (Mine)
            
            // fn cancel current request and create a new promise
            var restartCanceler = function () {
                if (typeof canceler !== 'undefined') {
                    canceler.resolve();
                }
                canceler = $q.defer();
            }

            // fn load users
            var loadUsers = function () {
                //abort all previous request
                //applicationFactory.abortPendingRequests();

                if ($scope.holiday.viewOther.filter.name === '') {
                    $scope.holiday.viewOther.onLoadingUser = false;
                    $scope.holiday.viewOther.userList = workContextSrv.getCurrentUser().following;
                    $scope.userTable.reload();
                } else {
                    $scope.holiday.viewOther.onLoadingUser = true;
                    userSrv.searchUser({ nameToSearch: $scope.holiday.viewOther.filter.name }).then(function (dt) {
                        $scope.holiday.viewOther.onLoadingUser = false;
                        $scope.holiday.viewOther.userList = dt;
                        $scope.userTable.reload();
                    }, function () {
                        $scope.holiday.viewOther.onLoadingUser = false;
                        // Show error message
                        myAlert.pop('e', 'Error!', 'There was an error occurred while searching users');
                    });
                }
            }

            // fn load current user holiday
            var loadData = function () {
                // get holidays
                holidaySrv.getUserHolidays(username, $scope.holiday.submit.gotYears[currentYearIndex - 1])
                   .then(function (dt) {
                       //add holidays to list
                       angular.forEach(dt, function (data) {
                           angular.forEach(data.holiday, function (date) {
                               //convert server date to date object
                               var dateObject = helperSrv.convertServerDateToDateObject(date);
                               //convert date object to formated date
                               var dateFormated = helperSrv.convertDateObjectToFormatedString(dateObject);
                               $scope.holiday.submit.markedDates.push({ date: dateFormated, note: "datetype-" + holidayFactory.getDateStatus(data) });
                           });
                       });
                       if (currentYearIndex == $scope.holiday.submit.gotYears.length) {
                           // hide preloading
                           applicationFactory.preLoading.show(false);
                           // show calendar
                           $scope.holiday.displayCalendar = true;
                       } else {
                           currentYearIndex++;
                           loadData();
                       }
                   }, function () {
                       // Show error message
                       myAlert.pop('e', 'Error!', 'There was an error occurred while getting user holidays');
                   });
            }

            // set active tab
            $scope.currentTab = "mine";                                             // Current tab (Mine, Other)

            // fn change tab
            $scope.changeTab = function (tab) {
                $scope.currentTab = tab;
                if (tab != 'mine') {
                    loadUsers();
                }
            };

            // check current tab
            $scope.isTab = function (tab) {
                return $scope.currentTab == tab;
            };

            // setup model
            $scope.holiday = {
                onSubmitting: false,                                                // On holiday submit state 
                displayCalendar: false,                                             // Display calendar state
            };

            // view other model
            $scope.holiday.viewOther = {
                // Filter view model
                filter: {
                    name: ''
                },
                onLoadingUser: false,                                               // On Loading user state
                userList: [],                                                       // Users array
                onViewDetail: false,                                                // On View user details state
                selectedUser: '',                                                   // Selected user
                selectedDates: [],                                                  // Selected dates
                gotYears: [],                                                       // Requested years to get user holidays
                firstLoad: true                                                     // First load state (Prevent callback from 'Watch')
            };

            // setup table
            $scope.userTable = new ngTableParams({                                  // ng-table create
                page: 1,                                                            // Page index
                count: 10,                                                          // Page size
                filter: {
                    //userName: 'M'       // initial filter
                }
            }, {
                counts: [],
                total: $scope.holiday.viewOther.userList.length, // length of data
                getData: function ($defer, params) {
                    // use build-in angular filter
                    params.total($scope.holiday.viewOther.userList.length); // set total for recalc pagination
                    $defer.resolve($scope.holiday.viewOther.userList.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                },
                $scope: { $data: {} }
            });

            // watch user typing name
            $scope.$watch("holiday.viewOther.filter.name", function () {
                loadUsers();
            });

            // setup submit model
            $scope.holiday.submit = {
                selectedDates: [],                                                  // Selected dates array
                markedDates: [],                                                    // Marked dates arrays 
                gotYears: [],                                                       // Years arrays
                note: '',                                                           // Note
                success: false,                                                     // Submit holiday success state
                successMessage: '',                                                 // Submit holiday success message
                hasError: false,                                                    // Submit holiday error state
                errorMEssage: ''                                                    // Submit holiday error message
            };

            // collect years to request get user holidays to display to calendar
            $scope.holiday.submit.gotYears.push(new Date().getFullYear());
            var lastDate = new Date(new Date().setMonth(new Date().getMonth() + numerOfDatePicker[1] - 1));
            if ($scope.holiday.submit.gotYears.indexOf(lastDate.getFullYear()) === -1) {
                $scope.holiday.submit.gotYears.push(lastDate.getFullYear());
            }

            // submit holiday
            $scope.submitHoliday = function (submit) {
                //hide message
                $scope.holiday.submit.success = false;
                $scope.holiday.submit.successMessage = '';
                $scope.holiday.submit.hasError = false;
                $scope.holiday.submit.errorMessage = '';
                //if user selected dates
                if ($scope.holiday.submit.selectedDates.length) {
                    //cancel pending request
                    restartCanceler();
                    //declare dates array
                    var dates = [];
                    //add selected date to array
                    angular.forEach($scope.holiday.submit.selectedDates, function (dt) {
                        //convert formated date to date object
                        var dateObject = helperSrv.convertFormatedStringToDateObject(dt.date);
                        //convert date object to server date
                        var serverDate = helperSrv.convertDateObjectToServerDate(dateObject);
                        dates.push(serverDate);
                    });
                    //show loading
                    $scope.holiday.onSubmitting = true;
                    //post request apply holiday
                    holidaySrv.applyHolidays(username, dates, canceler.promise)
                        .then(function (dt) {
                            //if success
                            if (angular.isDefined(dt.holidayId)) {
                                //create holiday workflow data
                                var workflowData = {
                                    params: {
                                        woid: '',
                                        wcode: 'HolidayWf'
                                    },
                                    data: [{
                                        "name": "UserName",
                                        "value": username,
                                        "type": "String"
                                    }, {
                                        "name": "UserNote",
                                        "value": submit.note.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, "<br/>"),
                                        "type": "String"
                                    }, {
                                        "name": "HolidayId",
                                        "value": dt.holidayId,
                                        "type": "String"
                                    }]
                                };
                                //post request start holiday workflow
                                holidaySrv.startHolidayWorkflow(workflowData.data, workflowData.params)
                                    .then(function () {
                                        //show message
                                        $scope.holiday.submit.success = true;
                                        $scope.holiday.submit.successMessage = 'Your Holidays was submitted';

                                        //mark submitted date to calendar
                                        $scope.holiday.submit.selectedDates.forEach(function (sd) {
                                            $scope.holiday.submit.markedDates.push({ date: sd.date, note: "datetype-1" });
                                        });

                                        //remove selected date on list
                                        $scope.holiday.submit.selectedDates = [];
                                        //remove notes
                                        $scope.holiday.submit.note = '';
                                        //remove selected date on calendar

                                        $scope.$broadcast('RESET.CALENDAR');
                                        //hide loading

                                        $scope.holiday.onSubmitting = false;
                                    }, function () {
                                        $scope.holiday.submit.hasError = true;
                                        $scope.holiday.submit.errorMessage = "Cannot start workflow";
                                        $scope.holiday.onSubmitting = false;
                                    });
                            } else {
                                //show error if result dont have holidayId value
                                $scope.holiday.submit.hasError = true;
                                $scope.holiday.submit.errorMessage = dt.message;
                                $scope.holiday.onSubmitting = false;
                            }
                        }, function (dter) {
                            //show error
                            $scope.holiday.submit.hasError = true;
                            $scope.holiday.submit.errorMessage = "Cannot Apply holiday";
                            $scope.holiday.onSubmitting = false;
                        });
                } else {
                    $scope.holiday.submit.hasError = true;
                    $scope.holiday.submit.errorMessage = "Please select at least 1 date";
                    $scope.holiday.onSubmitting = false;
                }
            };

            // back to list
            $scope.backToList = function () {
                currentOtherYearIndex = 1;
                //hide user holiday
                $scope.holiday.viewOther.onViewDetail = false;
                //set selected user to empty
                $scope.holiday.viewOther.selectedUser = '';
                //hide loading
                $scope.holiday.viewOther.onLoadingUser = false;
            };

            // view other user holiday
            $scope.viewUserHoliday = function (user) {
                //set current year
                // hide user list, show user holiday
                $scope.holiday.viewOther.onViewDetail = true;

                // show loading
                $scope.holiday.viewOther.onLoadingUser = true;

                // cancel pending request
                applicationFactory.abortPendingRequests();

                // reset the the year
                currentOtherYearIndex = 1;

                // clear selected dates
                $scope.holiday.viewOther.selectedDates = [];
                // clear selected years
                $scope.holiday.viewOther.gotYears = [];
                // add current year to array
                $scope.holiday.viewOther.gotYears.push(new Date().getFullYear());

                var lastDate2 = new Date(new Date().setMonth(new Date().getMonth() + numerOfDatePicker[1] - 1));
                if ($scope.holiday.viewOther.gotYears.indexOf(lastDate2.getFullYear()) === -1) {
                    $scope.holiday.viewOther.gotYears.push(lastDate2.getFullYear());
                }

                // set selected user
                $scope.holiday.viewOther.selectedUser = user.userName;
            };

            // load data
            loadData();
        }
    ]);