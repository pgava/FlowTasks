flowTasksApp
    .controller('ReportCtrl', ['$scope',
        function ($scope) {

        }
    ])
    .filter('getReportDataByUserAndTask', function () {
        return function (iptList, iptStr,task) {
            for (var i = 0; i < iptList.length; i++) {
                if (iptList[i].user == iptStr && iptList[i].task == task) return iptList[i];
            }
            return null;
        }
    });