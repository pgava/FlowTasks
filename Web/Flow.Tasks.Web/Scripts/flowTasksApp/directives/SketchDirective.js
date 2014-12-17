flowTasksApp
    .directive('loadSketch', [
        'ApplicationFactory', '$timeout', '$stateParams', function (applicationFactory, $timeout, $stateParams) {
            return {
                restrict: "A",
                link: function (scope) {
                    //scope.hasWorkflow = false;
                    // Hide preloading text
                    applicationFactory.preLoading.show(false);

                    if ($stateParams.workflow != null) {
                        Loading($stateParams.workflow);
                    } else {
                        Loading("");
                    }

                    //$timeout(function () {
                    //    scope.hasWorkflow = true;
                    //}, 500);
                }
            }
        }
    ])
    // View port resizing
    .directive('viewPortResize', [
        function () {
            return {
                restrict: 'A',
                link: function (scope, elem, attrs) {

                    //var resizeViewport = function () {
                    //    angular.element('#chart #viewport').css({ height: angular.element('#chart .jspContainer').height() - 50 + 'px' });
                    //}

                    //resizeViewport();

                    //$(window).resize(function () {
                    //    resizeViewport();
                    //});
                }
            }
        }
    ])
.directive('clearOldData', [function () {
    return {
        link: function () {
            jQuery(document).trigger("onChangingPage");
        }
    };
}])