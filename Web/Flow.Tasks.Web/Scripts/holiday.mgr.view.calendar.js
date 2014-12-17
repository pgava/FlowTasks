var holiday = {
    numerOfDatePicker: [1, 3],
    dateFormat: appConfig.dateFormat,
    gotYears: [],
    markedDates: [],
    selectedDates: [],
    xhr: null,
    apiGetUserHoliday: '../api/users/holiday',
    userName: jQuery("#currentUserName").val(),
    currentYearIndex: 1,
    previousWindowWidth: jQuery(window).width()
};
var otherHoliday = {
    userTable: {
        api: '../api/users',
        dataSource: [],
        bind: bindUserTable,
        xhr: null,
        loading: function (show) {
            if (show) {
                jQuery("#loader").css('visibility', 'visible');
            } else {
                jQuery("#loader").css('visibility', 'hidden');
            }
        }
    },
    showDetail: function (show) {
        jQuery("#searchUserTable").hide();
        jQuery("#viewOtherDetail").hide();
        if (show) {
            jQuery("#viewOtherDetail").show();
        } else {
            jQuery("#searchUserTable").show();
        }
    },
    numerOfDatePicker: [1, 3],
    dateFormat: appConfig.dateFormat,
    markedDates: [],
    selectedUser: '',
    gotYears: [],
    onLoading: false,
    xhr: null,
    apiGetUserHoliday: '../api/users/holiday',
    currentOtherYearIndex: 1
};

function getDateStatus(data) {
    if (data.type === 'Public') {
        return 3;
    }
    return data.status;
}

//load current user holiday
function firstLoad() {
    holiday.xhr = $.ajax({
        type: "GET",
        url: holiday.apiGetUserHoliday + "/" + holiday.userName + "?year=" + holiday.gotYears[holiday.currentYearIndex - 1],
        //data: "name=John&location=Boston",
        success: function (dt) {
            //add holidays to list
            jQuery.each(dt, function (idx, data) {
                jQuery.each(data.holiday, function (idx2, date) {
                    //convert server date to date object
                    var dateObject = convertServerDateToDateObject(date);
                    //convert date object to formated date
                    var dateFormated = convertDateObjectToFormatedString(dateObject);
                    holiday.markedDates.push({ date: dateFormated, note: "datetype-" + getDateStatus(data) });
                });
            });
            if (holiday.currentYearIndex == holiday.gotYears.length) {
                //render calendar
                renderCalendar();
            } else {
                holiday.currentYearIndex++;
                firstLoad();
            }
        }
    });
}

function convertDateObjectToFormatedString(date) {
    return moment(date).format(appConfig.momentDateFormat);
}
function convertFormatedStringToDateObject(dateString) {
    return moment(dateString, appConfig.momentDateFormat).toDate();
}
function convertServerDateToDateObject(serverDate) {
    return new Date(serverDate);
}
function convertDateObjectToServerDate(date) {
    return moment(date).format(appConfig.momentServerDateFormat);
}

var changeMonthYear = function (year, month, inst) {
    var dateObj = new Date(inst.selectedYear, inst.selectedMonth + holiday.numerOfDatePicker[1], inst.selectedDay);

    if (holiday.gotYears.indexOf(year) === -1) {
        reRenderCalendar(year);
    }

    if (holiday.gotYears.indexOf(dateObj.getFullYear()) === -1) {
        reRenderCalendar(dateObj.getFullYear());
    }
};

var beforeShowDay = function (date) {
    var isWhat = '';
    var userHolidayClass = '';
    var toolTip = '';

    var formatedDate = convertDateObjectToFormatedString(date);

    //disable Weekends
    if (date.getDay() == 0 || date.getDay() == 6) {
        isWhat = 'weekend';
    }

    //highlight user holiday
    for (var i = 0; i < holiday.markedDates.length; i++) {
        if (holiday.markedDates[i].date === formatedDate) {
            isWhat = 'userholiday';
            userHolidayClass = holiday.markedDates[i].note;
            toolTip = holiday.markedDates[i].note === 'datetype-2' ? 'Approved' :
                holiday.markedDates[i].note === 'datetype-1' ? 'Submitted' :
                holiday.markedDates[i].note === 'datetype-0' ? 'Rejected' :
                holiday.markedDates[i].note === 'datetype-3' ? 'Public' : '';
        }
    }

    var className = isWhat === 'weekend' ? 'weekend-date' : isWhat === 'holiday' ? 'holiday-date' : isWhat === 'userholiday' ? userHolidayClass : '';

    return [false, className, toolTip];
};

function clearCalendar() {
    jQuery("#holiday-calendar").removeClass("hasDatepicker");
    jQuery("#holiday-calendar").html('');
}

function reRenderCalendar(year) {
    holiday.gotYears.push(year);
    jQuery("#holiday-calendar").hide();

    holiday.xhr = $.ajax({
        type: "GET",
        url: holiday.apiGetUserHoliday + "/" + holiday.userName + "?year=" + year,
        //data: "name=John&location=Boston",
        success: function (dt) {
            //add holidays to list
            jQuery.each(dt, function (idx, data) {
                jQuery.each(data.holiday, function (idx2, date) {
                    //convert server date to date object
                    var dateObject = convertServerDateToDateObject(date);
                    //convert date object to formated date
                    var dateFormated = convertDateObjectToFormatedString(dateObject);
                    holiday.markedDates.push({ date: dateFormated, note: "datetype-" + getDateStatus(data) });
                });
            });
            //show calendar
            jQuery("#holiday-calendar").show();
            jQuery("#holiday-calendar").parent().find('.fa-spinner').remove();
        }
    });
}

function renderCalendar() {
    if (jQuery(window).width() <= 560) { //mobile portrait - 1 calendar
        holiday.numerOfDatePicker = [1, 1];
    } else if (jQuery(window).width() <= 795) { //mobile landscape, tablet portrait - 2 calendar
        holiday.numerOfDatePicker = [1, 2];
    }
    else {
        holiday.numerOfDatePicker = [1, 3];
    }

    jQuery('#holiday-calendar').multiDatesPicker({
        dateFormat: holiday.dateFormat,
        addDates: [],
        numberOfMonths: holiday.numerOfDatePicker,
        onChangeMonthYear: changeMonthYear,
        beforeShowDay: beforeShowDay
    });
}

var validResize = function () {
    if (holiday.previousWindowWidth > jQuery(window).width()) {
        if (holiday.previousWindowWidth - jQuery(window).width() > 25) {
            holiday.previousWindowWidth = jQuery(window).width();
            return true;
        }
    } else {
        if (jQuery(window).width() - holiday.previousWindowWidth > 25) {
            holiday.previousWindowWidth = jQuery(window).width();
            return true;
        }
    }
    return false;
};

function bindUserTable() {
    var $table = jQuery("#tabs-other #searchUserTable #userTable tbody");
    $table.empty();
    jQuery.each(otherHoliday.userTable.dataSource, function (idx, dt) {
        $table.append('<tr class="tr-user" data-user="' + dt.userName + '"><td>' + dt.userName + '</td><td>' + dt.firstName + '</td><td>' + dt.lastName + '</td><td>' + dt.email + '</td></tr>');
    });
}

/* Others */

function clearCalendar2() {
    jQuery("#other-holiday-calendar").removeClass("hasDatepicker");
    jQuery("#other-holiday-calendar").html('');
}

function loadUserHoliday() {
    otherHoliday.xhr = $.ajax({
        type: "GET",
        url: otherHoliday.apiGetUserHoliday + "/" + otherHoliday.selectedUser + "?year=" + otherHoliday.gotYears[otherHoliday.currentOtherYearIndex - 1],
        //data: "name=John&location=Boston",
        success: function (dt) {
            //add holidays to list
            jQuery.each(dt, function (idx, data) {
                jQuery.each(data.holiday, function (idx2, date) {
                    //convert server date to date object
                    var dateObject = convertServerDateToDateObject(date);
                    //convert date object to formated date
                    var dateFormated = convertDateObjectToFormatedString(dateObject);
                    otherHoliday.markedDates.push({ date: dateFormated, note: "datetype-" + getDateStatus(data) });
                });
            });

            if (otherHoliday.currentOtherYearIndex >= otherHoliday.gotYears.length) {
                otherHoliday.currentOtherYearIndex = 1;
                //remove loading
                otherHoliday.onLoading = false;
                //render calendar
                renderCalendar2(true);
            } else {
                otherHoliday.currentOtherYearIndex++;
                loadUserHoliday();
            }
        }
    });
}

var changeMonthYear2 = function (year, month, inst) {
    var dateObj = new Date(inst.selectedYear, inst.selectedMonth + otherHoliday.numerOfDatePicker[1], inst.selectedDay);

    if (otherHoliday.gotYears.indexOf(year) === -1) {
        reRenderCalendar2(year);
    }

    if (otherHoliday.gotYears.indexOf(dateObj.getFullYear()) === -1) {
        reRenderCalendar2(dateObj.getFullYear());
    }
};

var beforeShowDay2 = function (date) {
    var isWhat = '';
    var userHolidayClass = '';
    var toolTip = '';

    var formatedDate = convertDateObjectToFormatedString(date);

    //disable Weekends
    if (date.getDay() == 0 || date.getDay() == 6) {
        isWhat = 'weekend';
    }

    //highlight user holiday
    for (var i = 0; i < otherHoliday.markedDates.length; i++) {
        if (otherHoliday.markedDates[i].date === formatedDate) {
            isWhat = 'userholiday';
            userHolidayClass = otherHoliday.markedDates[i].note;
            toolTip = otherHoliday.markedDates[i].note === 'datetype-1' ? 'Approved' :
                otherHoliday.markedDates[i].note === 'datetype-0' ? 'Submitted' :
                otherHoliday.markedDates[i].note === 'datetype-0' ? 'Rejected' :
                otherHoliday.markedDates[i].note === 'datetype-2' ? 'Public' : '';
        }
    }

    var className = isWhat === 'weekend' ? 'weekend-date' : isWhat === 'holiday' ? 'holiday-date' : isWhat === 'userholiday' ? userHolidayClass : '';

    return [false, className, toolTip];
};

function reRenderCalendar2(year) {
    otherHoliday.gotYears.push(year);
    jQuery("#other-holiday-calendar").hide();

    otherHoliday.xhr = $.ajax({
        type: "GET",
        url: otherHoliday.apiGetUserHoliday + "/" + otherHoliday.selectedUser + "?year=" + year,
        //data: "name=John&location=Boston",
        success: function (dt) {
            //add holidays to list
            jQuery.each(dt, function (idx, data) {
                jQuery.each(data.holiday, function (idx2, date) {
                    //convert server date to date object
                    var dateObject = convertServerDateToDateObject(date);
                    //convert date object to formated date
                    var dateFormated = convertDateObjectToFormatedString(dateObject);
                    otherHoliday.markedDates.push({ date: dateFormated, note: "datetype-" + getDateStatus(data) });
                });
            });

            //show calendar
            jQuery("#other-holiday-calendar").show();
        }
    });
}

function renderCalendar2() {
    if (jQuery(window).width() <= 560) { //mobile portrait - 1 calendar
        otherHoliday.numerOfDatePicker = [1, 1];
    } else if (jQuery(window).width() <= 795) { //mobile landscape, tablet portrait - 2 calendar
        otherHoliday.numerOfDatePicker = [1, 2];
    }
    else {
        otherHoliday.numerOfDatePicker = [1, 3];
    }

    jQuery('#other-holiday-calendar').multiDatesPicker({
        dateFormat: otherHoliday.dateFormat,
        addDates: [],
        numberOfMonths: otherHoliday.numerOfDatePicker,
        onChangeMonthYear: changeMonthYear2,
        beforeShowDay: beforeShowDay2,
    });
}

/* End Other */


//re-create calendar fit with window width
jQuery(window).resize(function () {
    if (validResize()) {
        clearCalendar();
        renderCalendar();

        clearCalendar2();
        renderCalendar2();
    }
});

jQuery(document).ready(function () {
    //Tabs
    $("#tabs").tabs();

    //render current user calendar
    setTimeout(function () {
        //cancel pending request
        if (holiday.xhr != null) {
            holiday.xhr.abort();
        }
        //clear calendar
        clearCalendar();
        //get holidays
        holiday.gotYears.push(new Date().getFullYear());
        var lastDate = new Date(new Date().setMonth(new Date().getMonth() + holiday.numerOfDatePicker[1] - 1));
        if (holiday.gotYears.indexOf(lastDate.getFullYear()) === -1) {
            holiday.gotYears.push(lastDate.getFullYear());
        }
        //clear selected dates
        holiday.markedDates = [];
        //alert("");
        firstLoad();
    }, 300);

    /* View other */
    jQuery("#searchName").keyup(function () {
        if (otherHoliday.userTable.xhr != null) {
            otherHoliday.userTable.xhr.abort();
        }
        if (jQuery.trim(jQuery(this).val()) === '') {
            otherHoliday.userTable.dataSource = [];
            otherHoliday.userTable.bind();
        } else {
            otherHoliday.userTable.loading(true);
            otherHoliday.userTable.xhr = $.ajax({
                type: "GET",
                url: otherHoliday.userTable.api + "?nameToSearch=" + jQuery.trim(jQuery(this).val()),
                //data: "name=John&location=Boston",
                success: function (dt) {
                    otherHoliday.userTable.loading(false);
                    otherHoliday.userTable.dataSource = dt;
                    otherHoliday.userTable.bind();
                }
            });
        }
    });

    //view other detail
    jQuery(document).on("click", "#tabs-other #searchUserTable #userTable tbody tr", function () {
        //set selected user
        otherHoliday.selectedUser = jQuery(this).attr("data-user");
        //show loading
        otherHoliday.onLoading = true;
        //cancel pending request
        if (otherHoliday.xhr != null) {
            otherHoliday.xhr.abort();
        }
        //clear calendar
        clearCalendar2();
        otherHoliday.currentOtherYearIndex = 1;
        //remove selected date
        otherHoliday.markedDates = [];
        otherHoliday.gotYears = [];
        otherHoliday.gotYears.push(new Date().getFullYear());
        var lastDate = new Date(new Date().setMonth(new Date().getMonth() + otherHoliday.numerOfDatePicker[1] - 1));
        if (otherHoliday.gotYears.indexOf(lastDate.getFullYear()) === -1) {
            otherHoliday.gotYears.push(lastDate.getFullYear());
        }
        //load calendar
        loadUserHoliday();
        //show detail
        otherHoliday.showDetail(true);

        jQuery("#otherUserName").html(otherHoliday.selectedUser);
    });

    //back to user list
    jQuery("#btnBackToList").click(function () {
        otherHoliday.showDetail(false);
        otherHoliday.clearCalendar2 = '';
        clearCalendar2();
    });
});

