flowTasksApp
    // Vier current user holidays
    .directive('holidayCalendar', ['globalConfig', 'HolidayFactory', 'helperSrv', 'HolidaySrv', 'ApplicationFactory', 'myAlert', function (globalConfig, holidayFactory, helperSrv, holidaySrv, applicationFactory, myAlert) {
        return {
            restrict: 'A',
            scope: {
                holiday: '=holidayCalendar',

            },
            link: function (scope, elem) {

                var username = applicationFactory.userData.userName;                // get current username     
                var numerOfDatePicker = [1, 3];                                     // Number of calendars

                if (jQuery(window).width() <= 375) {                                // mobile portrait - 1 calendar
                    numerOfDatePicker = [1, 1];                                     // 1 calendar
                } else if (jQuery(window).width() <= 601) {                         // mobile landscape, tablet portrait - 2 calendar
                    numerOfDatePicker = [1, 2];                                     // 2 calendars
                }
                else {
                    numerOfDatePicker = [1, 3];                                     // 3 calendars
                }

                // fn re-render calendar
                function reRenderCalendar(year) {
                    // add year to array
                    scope.holiday.submit.gotYears.push(year);
                    // Hide calendar
                    elem.hide();
                    // Show loading icon
                    elem.parent().append('<i class="custom-spinning fa-2x" style="position: relative; left: 48%; color: rgb(44, 55, 66);"></i>');
                    // Do request get user holiday by year
                    holidaySrv.getUserHolidays(username, year)
                   .then(function (dt) {
                       //add holidays to list
                       angular.forEach(dt, function (data) {
                           angular.forEach(data.holiday, function (date) {
                               //convert server date to date object
                               var dateObject = helperSrv.convertServerDateToDateObject(date);
                               //convert date object to formated date
                               var dateFormated = helperSrv.convertDateObjectToFormatedString(dateObject);
                               scope.holiday.submit.markedDates.push({ date: dateFormated, note: "datetype-" + holidayFactory.getDateStatus(data) });
                           });
                       });
                       //show calendar
                       elem.show();
                       // Hide loading icon
                       elem.parent().find('.custom-spinning').remove();
                   }, function () {
                       // Show calendar
                       elem.show();
                       // Hide loading icon
                       elem.parent().find('.custom-spinning').remove();
                       // Show error message
                       myAlert.pop('e', 'Error!', 'There was an error occurred while loading user holidays');
                   });
                }

                // watch display calendar
                scope.$watch('holiday.displayCalendar', function () {
                    if (scope.holiday.displayCalendar) {
                        elem.multiDatesPicker({
                            nextText: '<i class="fa fa-chevron-right"></i>',
                            prevText: '<i class="fa fa-chevron-left"></i>',
                            dateFormat: globalConfig.dateFormat,
                            addDates: [], //set selected date
                            numberOfMonths: numerOfDatePicker, // rows,columns
                            onSelect: function (date) {
                                var exist = false;
                                for (var i = 0; i < scope.holiday.submit.selectedDates.length; i++) {
                                    if (scope.holiday.submit.selectedDates[i].date === date) {
                                        scope.holiday.submit.selectedDates.splice(i, 1);
                                        exist = true;
                                        break;
                                    }
                                }

                                if (!exist) {
                                    scope.holiday.submit.selectedDates.push({ date: date, note: "datetype-2" });
                                }
                            },
                            onChangeMonthYear: function (year, month, inst) {

                                var dateObj = new Date(inst.selectedYear, inst.selectedMonth + numerOfDatePicker[1], inst.selectedDay);

                                if (scope.holiday.submit.gotYears.indexOf(year) === -1) {
                                    reRenderCalendar(year);
                                }

                                if (scope.holiday.submit.gotYears.indexOf(dateObj.getFullYear()) === -1) {
                                    reRenderCalendar(dateObj.getFullYear());
                                }

                            },
                            beforeShowDay: function (date) {
                                var show = true;
                                var isWhat = '';
                                var userHolidayClass = '';
                                var toolTip = '';

                                var formatedDate = helperSrv.convertDateObjectToFormatedString(date);

                                // disable Weekends
                                if (date.getDay() == 0 || date.getDay() == 6) {
                                    isWhat = 'weekend';
                                    show = false;
                                }

                                // old days
                                var toDay = new Date();
                                if (date < toDay.setDate(toDay.getDate() - 1)) {
                                    isWhat = 'oldday';
                                    show = false;
                                }

                                // highlight user holidays
                                for (var i = 0; i < scope.holiday.submit.markedDates.length; i++) {
                                    if (scope.holiday.submit.markedDates[i].date === formatedDate) {
                                        show = false;
                                        isWhat = 'userholiday';
                                        userHolidayClass = scope.holiday.submit.markedDates[i].note;
                                        toolTip = scope.holiday.submit.markedDates[i].note === 'datetype-2' ? 'Approved' :
                                            scope.holiday.submit.markedDates[i].note === 'datetype-1' ? 'Submitted' :
                                            scope.holiday.submit.markedDates[i].note === 'datetype-0' ? 'Rejected' :
                                            scope.holiday.submit.markedDates[i].note === 'datetype-3' ? 'Public' : '';
                                    }
                                }

                                var className = isWhat === 'weekend' ? 'weekend-date' : isWhat === 'oldday'
                                                                     ? 'old-date' : isWhat === 'holiday'
                                                                     ? 'holiday-date' : isWhat === 'userholiday'
                                                                     ? userHolidayClass : '';

                                return [show, className, toolTip];
                            },
                        });
                    }
                });

                scope.$on('RESET.CALENDAR', function () {
                   angular.element("#holiday-calendar").multiDatesPicker('resetDates', 'picked');
                });
            }
        }
    }])

    // View other user holidays
    .directive('otherHolidayCalendar', ['HolidaySrv', 'helperSrv', 'globalConfig', 'myAlert', 'HolidayFactory', function (holidaySrv, helperSrv, globalConfig, myAlert, holidayFactory) {
        return {
            restrict: "A",
            scope: {
                holiday: '=otherHolidayCalendar'
            },
            link: function (scope, elem) {
                var numerOfDatePicker = [1, 3];
                var currentOtherYearIndex = 1;

                // fn clear calendar
                function clearCalendar() {
                    elem.removeClass("hasDatepicker");
                    elem.html('');
                }

                // fn re-render calendar
                function reRenderCalendar(year) {
                    // add year to array
                    scope.holiday.viewOther.gotYears.push(year);
                    // Hide calendar
                    elem.hide();
                    // Show loading icon
                    elem.parent().append('<i class="custom-spinning fa-2x" style="position: relative; left: 48%; color: rgb(44, 55, 66);"></i>');
                    // Do request get user holidays by year
                    holidaySrv.getUserHolidays(scope.holiday.viewOther.selectedUser, year)
                   .then(function (dt) {
                       // add holidays to list
                       angular.forEach(dt, function (data) {
                           angular.forEach(data.holiday, function (date) {
                               //convert server date to date object
                               var dateObject = helperSrv.convertServerDateToDateObject(date);
                               //convert date object to formated date
                               var dateFormated = helperSrv.convertDateObjectToFormatedString(dateObject);
                               scope.holiday.viewOther.selectedDates.push({ date: dateFormated, note: "datetype-" + holidayFactory.getDateStatus(data) });
                           });
                       });
                       // show calendar
                       elem.show();
                       // hide loading icon
                       elem.parent().find('.custom-spinning').remove();
                   }, function () {
                       // show calendar
                       elem.show();
                       // hide loading icon
                       elem.parent().find('.custom-spinning').remove();
                       // Show error message
                       myAlert.pop('e', 'Error!', 'There was an error occurred while loading user holidays');
                   });
                }

                // fn render calendar
                function renderCalendar(disabledAll) {
                    if (jQuery(window).width() <= 375) {                    // mobile portrait - 1 calendar
                        numerOfDatePicker = [1, 1];                         // 1 calendar
                    } else if (jQuery(window).width() <= 601) {             // mobile landscape,, tablet portrait - 2 calendar
                        numerOfDatePicker = [1, 2];                         // 2 calendars
                    }
                    else {
                        numerOfDatePicker = [1, 3];                         // 3 calendars
                    }

                    // Render calendar
                    elem.multiDatesPicker({
                        nextText: '<i class="fa fa-chevron-right"></i>', prevText: '<i class="fa fa-chevron-left"></i>',
                        dateFormat: globalConfig.dateFormat,
                        //defaultDate:"85-02-19"
                        addDates: [], //set selected date
                        numberOfMonths: numerOfDatePicker, // rows,columns
                        onChangeMonthYear: function (year, month, inst) {
                            if (scope.holiday.viewOther.gotYears.indexOf(year) === -1) {
                                reRenderCalendar(year);
                            }

                            var dateObj = new Date(inst.selectedYear, inst.selectedMonth + numerOfDatePicker[1], inst.selectedDay);

                            if (scope.holiday.viewOther.gotYears.indexOf(dateObj.getFullYear()) === -1) {
                                reRenderCalendar(dateObj.getFullYear());
                            }
                        },
                        //addDisabledDates: [today.setDate(1), today.setDate(3)], // disable dates	
                        beforeShowDay: function (date) {
                            var show = true;
                            var isWhat = '';
                            var userHolidayClass = '';
                            var toolTip = '';

                            var formatedDate = helperSrv.convertDateObjectToFormatedString(date);

                            //disable Weekends
                            if (date.getDay() == 0 || date.getDay() == 6) {
                                isWhat = 'weekend';
                                show = false;
                            }

                            //highlight user holiday
                            for (var i = 0; i < scope.holiday.viewOther.selectedDates.length; i++) {
                                // mm/dd/yyyy -? yyyy-mm-dd
                                //var dateObject = scope.holiday.submit.selectedDates[i].date.replace(/(\d{1,2})\/(\d{1,2})\/(\d{4})/g, function ($0, $1, $2, $3) { return $3 + "/" + $1 + "/" + $2; });
                                if (scope.holiday.viewOther.selectedDates[i].date === formatedDate) {
                                    isWhat = 'userholiday';
                                    userHolidayClass = scope.holiday.viewOther.selectedDates[i].note;
                                    toolTip =
                                        scope.holiday.viewOther.selectedDates[i].note === 'datetype-2' ? 'Approved' :
                                        scope.holiday.viewOther.selectedDates[i].note === 'datetype-1' ? 'Submitted' :
                                        scope.holiday.viewOther.selectedDates[i].note === 'datetype-2' ? 'Rejected' :
                                        scope.holiday.viewOther.selectedDates[i].note === 'datetype-3' ? 'Public' : '';
                                }
                            }

                            var className = isWhat === 'weekend' ? 'weekend-date' : isWhat === 'holiday' ? 'holiday-date' : isWhat === 'userholiday' ? userHolidayClass : '';

                            if (disabledAll) {
                                return [false, className, toolTip];
                            }
                            return [show, className, toolTip];
                        },
                    });
                }

                // fn load user holidays
                var loadUserHoliday = function () {
                    //get user holidays
                    holidaySrv.getUserHolidays(scope.holiday.viewOther.selectedUser,
                        scope.holiday.viewOther.gotYears[currentOtherYearIndex - 1])
                        .then(function (dt) {
                            //add holiday to list
                            angular.forEach(dt, function (data) {
                                angular.forEach(data.holiday, function (date) {
                                    //convert server date to date object
                                    var dateObject = helperSrv.convertServerDateToDateObject(date);
                                    //convert date object to formated date
                                    var dateFormated = helperSrv.convertDateObjectToFormatedString(dateObject);
                                    scope.holiday.viewOther.selectedDates.push({ date: dateFormated, note: "datetype-" + holidayFactory.getDateStatus(data) });
                                });
                            });

                            if (currentOtherYearIndex >= scope.holiday.viewOther.gotYears.length) {
                                currentOtherYearIndex = 1;

                                // remove first load state
                                scope.holiday.viewOther.firstLoad = false;

                                // remove loading
                                scope.holiday.viewOther.onLoadingUser = false;

                                // render calendar
                                renderCalendar(true);
                            } else {
                                currentOtherYearIndex++;
                                loadUserHoliday();
                            }
                        }, function () {
                            // remove loading
                            scope.holiday.viewOther.onLoadingUser = false;
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while loading user holidays');
                        });
                }

                // Watch selected user changing
                scope.$watch('holiday.viewOther.selectedUser', function () {
                    // clear calendar
                    clearCalendar();

                    if (scope.holiday.viewOther.selectedUser != '') {
                        loadUserHoliday();
                    }
                });
            }
        }
    }])
