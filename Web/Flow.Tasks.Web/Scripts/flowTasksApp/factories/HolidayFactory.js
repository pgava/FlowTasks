flowTasksApp
    .factory('HolidayFactory', [function () {
       
        var holiday = {};

        // fn get date state (0 - Rejected, 1 - Submitted, 2 - Approved, 3 - Public )
        holiday.getDateStatus = function (data) {
            if (data.type === 'Public') {
                return 3;
            }
            return data.status;
        }

        return holiday;
    }])