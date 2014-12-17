flowTasksApp
    // Download workflow file
    .directive("downloadFileDashboard", function () {
        return {
            scope: {
                property: '='
            },
            restrict: 'E',
            replace: true,
            controller: ['$scope', function ($scope) {
                // fn download file
                $scope.saveFile = function (property) {
                    window.open('api/docs/' + property.value);
                };
            }],
            template: '<b class="pull-left col-sm-6 btnsavefile" ng-click="saveFile(property)"><i class="fa fa-file"></i> {{property.value}}</b>'
        };
    })
    // Select worklfow
    .directive('selectWorkflow', [function () {
        return {
            restrict: "A",
            scope: {
                select: '&selectWorkflow',
                workflow: '='
            },
            link: function (scope, elem, attrs) {
                elem.on("click", function () {
                    var isChecked = elem.is(":checked");
                    // do select
                    scope.select({ fl: scope.workflow, selected: isChecked });
                });

                scope.$on("$destroy", function () {
                    elem.off("click");
                });
            }
        }
    }])
    // View workflow details
    .directive('viewWorkflowDetails', [function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                elem.on("click", function (e) {
                    if (!angular.element(e.target).hasClass("expand-collapse-button")) {
                        // execute method
                        scope.$eval(attrs.viewWorkflowDetails);
                    }
                });

                scope.$on("$destroy", function () {
                    elem.off("click");
                });
            }
        }
    }])
    // Workflow row
    .directive('workflowRow', [function () {
        return {
            restrict: "A",
            templateUrl:'scripts/flowTasksApp/views/Dashboard/index.workflow.row.html'
        }
    }])
    // Workflow child
    .directive('workflowChild', [function () {
        return {
            restrict: "A",
            templateUrl:'scripts/flowTasksApp/views/Dashboard/index.workflow.row.child.html'
        }
    }])
    // Wofklow filter
    .directive('dashboardFilter',[function() {
        return {
            restrict: 'A',
            templateUrl: 'scripts/flowTasksApp/views/Dashboard/index.filter.html'
        }
    }])
    // Wofklow control (buttons)
    .directive('dashboardWorkflowControl', [function () {
        return {
            restrict: 'A',
            templateUrl: 'scripts/flowTasksApp/views/Dashboard/index.control.html'
        }
    }])
    // Workflow details
    .directive('dashboardWorkflowDetails', [function () {
        return {
            restrict: 'A',
            templateUrl: 'scripts/flowTasksApp/views/Dashboard/index.details.html'
        }
    }])
;