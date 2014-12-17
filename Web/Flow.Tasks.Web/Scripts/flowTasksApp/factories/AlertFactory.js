angular.module('myAlert', [])
.service('myAlert', ['$rootScope', function ($rootScope) {
    this.pop = function (type, title, body) {
        this.alert = {
            title: title,
            content: body,
            color: type == 's' ? "#739E73" : type == 'e' ? "#C46A6A" : "#3276B1",
            timeout: 4000
        };
        $rootScope.$broadcast('my-alert-new');
    };
}])
.directive("myAlertContainer", ['$rootScope', 'myAlert', function ($rootScope,myAlert) {
    return {
        restrict: "A",
        replace: false,
        scope: true, // creates an internal scope for this directive
        link: function (scope, elm, attrs) {

            $rootScope.$on('my-alert-new', function () {
                addAlertBox(myAlert.alert);
            });

            var addAlertBox = function (info) {
                jQuery.smallBox(info);
            }
        }
    }
}])