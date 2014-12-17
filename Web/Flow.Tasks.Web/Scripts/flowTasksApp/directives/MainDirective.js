flowTasksApp
    .directive("smartAlert", ['WorkContextSrv', '$state', 'UserSrv', '$timeout', 'webStorage', 'ApplicationFactory', '$rootScope', function (workContextSrv, $state, userSrv, $timeout, webStorage, applicationFactory, $rootScope) {
        return {
            restrict: 'E',
            replace: true,
            transclude: false,
            compile: function (element, attrs) {
                var html = "<a href='#' title='" + attrs.title + "'><i class='" + attrs.iconclass + "'></i></a>";
                var newElem = angular.element(html);
                element.replaceWith(newElem);
                return function (scope, elem, attrs, controller) {
                    if (attrs.type == 'logout') {
                        elem.on('click', function (e) {
                            e.preventDefault();
                            $.SmartMessageBox({
                                title: "<i class='fa fa-sign-out txt-color-orangeDark'></i> Logout <span class='txt-color-orangeDark'><strong>" + attrs.title + "</strong></span> ?",
                                content: $.logoutMSG || attrs.text,
                                buttons: '[No][Yes]'
                            }, function (buttonPressed) {
                                if (buttonPressed == "Yes") {
                                    userSrv.logout().then(function (dt, st) {
                                        if (dt == true) {
                                            // abort all pending requests after log out successful
                                            $rootScope.$broadcast('abortPendingRequest');
                                            // redirect to sign in page
                                            $state.go("signin");
                                            $rootScope.$broadcast("USER:LOGOUT");
                                        }
                                    }, function (dt, st) {
                                        // Go to sign in page
                                        $state.go("signin");
                                        $rootScope.$broadcast("USER:LOGOUT");
                                    });
                                }

                            });
                        });
                    } else if (type == 'callback') {

                    }

                };

            }
        };
    }])
    .directive("closeSidebar", [function () {
        return {
            restrict: 'E',
            replace: true,
            transclude: false,
            compile: function (element) {
                var html = "<a href='javascript:void(0);' title='Collapse Menu'><i class='fa fa-reorder'></i></a>";
                var newElem = angular.element(html);
                element.replaceWith(newElem);
                return function (scope, elem) {
                    elem.on('click', function (e) {
                        angular.element('body').toggleClass("hidden-menu");
                        e.preventDefault();
                    });
                };

            }
        };
    }])
    .directive("collapseSidebar", [function () {
        return {
            restrict: 'E',
            replace: true,
            transclude: false,
            compile: function (element) {
                var html = "<span class='minifyme'><i class='fa fa-arrow-circle-left hit'></i></span>";
                var newElem = angular.element(html);
                element.replaceWith(newElem);
                return function (scope, elem) {
                    elem.on('click', function (e) {
                        angular.element('body').toggleClass("minified");
                        angular.element(this).effect("highlight", {}, 500);
                        e.preventDefault();
                    });
                };

            }
        };
    }])
    .directive("sparksChart", ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            replace: true,
            template: '<div class="sparkline hidden-mobile hidden-xs">{{attrs.values}}</div>',
            link: function (scope, elem, attrs) {
                var tm = null;
                var oldv = 0;
                attrs.$observe("count", function (newv) {
                    $timeout.cancel(tm);
                    tm = $timeout(function () {
                        if (newv != oldv) {
                            oldv = newv;
                            createSpartChart();
                        }
                    }, 1000);
                });

                function createSpartChart() {
                    var $this = elem;
                    $this.html('');
                    $this.removeClass(attrs.color);
                    $this.addClass(attrs.color);
                    var sparklineType = attrs.type;
                    if (attrs.values == '') {
                        return;
                    }
                    // BAR CHART
                    if (sparklineType == 'bar') {
                        var barColor = $this.data('sparkline-bar-color') || $this.css('color') || '#0000f0', sparklineHeight = $this.data('sparkline-height') || '26px', sparklineBarWidth = $this.data('sparkline-barwidth') || 5, sparklineBarSpacing = $this.data('sparkline-barspacing') || 2, sparklineNegBarColor = $this.data('sparkline-negbar-color') || '#A90329', sparklineStackedColor = $this.data('sparkline-barstacked-color') || ["#A90329", "#0099c6", "#98AA56", "#da532c", "#4490B1", "#6E9461", "#990099", "#B4CAD3"];
                        $timeout(function () {
                            $this.sparkline('html', {
                                barColor: barColor,
                                type: sparklineType,
                                height: sparklineHeight,
                                barWidth: sparklineBarWidth,
                                barSpacing: sparklineBarSpacing,
                                stackedBarColor: sparklineStackedColor,
                                negBarColor: sparklineNegBarColor,
                                zeroAxis: 'false',
                                tooltipFormat: '{{offset:offset}}: {{value}}',
                                tooltipValueLookups: {
                                    'offset': attrs.offset.split(',')
                                }
                            });
                        }, 200);

                    }

                    //LINE CHART
                    if (sparklineType == 'line') {

                        var sparklineHeight = $this.data('sparkline-height') || '20px', sparklineWidth = $this.data('sparkline-width') || '90px', thisLineColor = $this.data('sparkline-line-color') || $this.css('color') || '#0000f0', thisLineWidth = $this.data('sparkline-line-width') || 1, thisFill = $this.data('fill-color') || '#c0d0f0', thisSpotColor = $this.data('sparkline-spot-color') || '#f08000', thisMinSpotColor = $this.data('sparkline-minspot-color') || '#ed1c24', thisMaxSpotColor = $this.data('sparkline-maxspot-color') || '#f08000', thishighlightSpotColor = $this.data('sparkline-highlightspot-color') || '#50f050', thisHighlightLineColor = $this.data('sparkline-highlightline-color') || 'f02020', thisSpotRadius = $this.data('sparkline-spotradius') || 1.5;
                        thisChartMinYRange = $this.data('sparkline-min-y') || 'undefined', thisChartMaxYRange = $this.data('sparkline-max-y') || 'undefined', thisChartMinXRange = $this.data('sparkline-min-x') || 'undefined', thisChartMaxXRange = $this.data('sparkline-max-x') || 'undefined', thisMinNormValue = $this.data('min-val') || 'undefined', thisMaxNormValue = $this.data('max-val') || 'undefined', thisNormColor = $this.data('norm-color') || '#c0c0c0', thisDrawNormalOnTop = $this.data('draw-normal') || false;

                        $this.sparkline('html', {
                            type: 'line',
                            width: sparklineWidth,
                            height: sparklineHeight,
                            lineWidth: thisLineWidth,
                            lineColor: thisLineColor,
                            fillColor: thisFill,
                            spotColor: thisSpotColor,
                            minSpotColor: thisMinSpotColor,
                            maxSpotColor: thisMaxSpotColor,
                            highlightSpotColor: thishighlightSpotColor,
                            highlightLineColor: thisHighlightLineColor,
                            spotRadius: thisSpotRadius,
                            chartRangeMin: thisChartMinYRange,
                            chartRangeMax: thisChartMaxYRange,
                            chartRangeMinX: thisChartMinXRange,
                            chartRangeMaxX: thisChartMaxXRange,
                            normalRangeMin: thisMinNormValue,
                            normalRangeMax: thisMaxNormValue,
                            normalRangeColor: thisNormColor,
                            drawNormalOnTop: thisDrawNormalOnTop

                        });

                    }

                    //PIE CHART
                    if (sparklineType == 'pie') {

                        var pieColors = $this.data('sparkline-piecolor') || ["#B4CAD3", "#4490B1", "#98AA56", "#da532c", "#6E9461", "#0099c6", "#990099", "#717D8A"], pieWidthHeight = $this.data('sparkline-piesize') || 90, pieBorderColor = $this.data('border-color') || '#45494C', pieOffset = $this.data('sparkline-offset') || 0;

                        $this.sparkline('html', {
                            type: 'pie',
                            width: pieWidthHeight,
                            height: pieWidthHeight,
                            tooltipFormat: '<span style="color: {{color}}">&#9679;</span> ({{percent.1}}%)',
                            sliceColors: pieColors,
                            offset: 0,
                            borderWidth: 1,
                            offset: pieOffset,
                            borderColor: pieBorderColor
                        });

                    }

                    //BOX PLOT
                    if (sparklineType == 'box') {

                        var thisBoxWidth = $this.data('sparkline-width') || 'auto', thisBoxHeight = $this.data('sparkline-height') || 'auto', thisBoxRaw = $this.data('sparkline-boxraw') || false, thisBoxTarget = $this.data('sparkline-targetval') || 'undefined', thisBoxMin = $this.data('sparkline-min') || 'undefined', thisBoxMax = $this.data('sparkline-max') || 'undefined', thisShowOutlier = $this.data('sparkline-showoutlier') || true, thisIQR = $this.data('sparkline-outlier-iqr') || 1.5, thisBoxSpotRadius = $this.data('sparkline-spotradius') || 1.5, thisBoxLineColor = $this.css('color') || '#000000', thisBoxFillColor = $this.data('fill-color') || '#c0d0f0', thisBoxWhisColor = $this.data('sparkline-whis-color') || '#000000', thisBoxOutlineColor = $this.data('sparkline-outline-color') || '#303030', thisBoxOutlineFill = $this.data('sparkline-outlinefill-color') || '#f0f0f0', thisBoxMedianColor = $this.data('sparkline-outlinemedian-color') || '#f00000', thisBoxTargetColor = $this.data('sparkline-outlinetarget-color') || '#40a020';

                        $this.sparkline('html', {
                            type: 'box',
                            width: thisBoxWidth,
                            height: thisBoxHeight,
                            raw: thisBoxRaw,
                            target: thisBoxTarget,
                            minValue: thisBoxMin,
                            maxValue: thisBoxMax,
                            showOutliers: thisShowOutlier,
                            outlierIQR: thisIQR,
                            spotRadius: thisBoxSpotRadius,
                            boxLineColor: thisBoxLineColor,
                            boxFillColor: thisBoxFillColor,
                            whiskerColor: thisBoxWhisColor,
                            outlierLineColor: thisBoxOutlineColor,
                            outlierFillColor: thisBoxOutlineFill,
                            medianColor: thisBoxMedianColor,
                            targetColor: thisBoxTargetColor

                        });

                    }

                    //BULLET
                    if (sparklineType == 'bullet') {

                        var thisBulletHeight = $this.data('sparkline-height') || 'auto', thisBulletWidth = $this.data('sparkline-width') || 2, thisBulletColor = $this.data('sparkline-bullet-color') || '#ed1c24', thisBulletPerformanceColor = $this.data('sparkline-performance-color') || '#3030f0', thisBulletRangeColors = $this.data('sparkline-bulletrange-color') || ["#d3dafe", "#a8b6ff", "#7f94ff"];

                        $this.sparkline('html', {
                            type: 'bullet',
                            height: thisBulletHeight,
                            targetWidth: thisBulletWidth,
                            targetColor: thisBulletColor,
                            performanceColor: thisBulletPerformanceColor,
                            rangeColors: thisBulletRangeColors

                        });

                    }

                    //DISCRETE
                    if (sparklineType == 'discrete') {

                        var thisDiscreteHeight = $this.data('sparkline-height') || 26, thisDiscreteWidth = $this.data('sparkline-width') || 50, thisDiscreteLineColor = $this.css('color'), thisDiscreteLineHeight = $this.data('sparkline-line-height') || 5, thisDiscreteThrushold = $this.data('sparkline-threshold') || 'undefined', thisDiscreteThrusholdColor = $this.data('sparkline-threshold-color') || '#ed1c24';

                        $this.sparkline('html', {
                            type: 'discrete',
                            width: thisDiscreteWidth,
                            height: thisDiscreteHeight,
                            lineColor: thisDiscreteLineColor,
                            lineHeight: thisDiscreteLineHeight,
                            thresholdValue: thisDiscreteThrushold,
                            thresholdColor: thisDiscreteThrusholdColor

                        });

                    }

                    //TRISTATE
                    if (sparklineType == 'tristate') {

                        var thisTristateHeight = $this.data('sparkline-height') || 26, thisTristatePosBarColor = $this.data('sparkline-posbar-color') || '#60f060', thisTristateNegBarColor = $this.data('sparkline-negbar-color') || '#f04040', thisTristateZeroBarColor = $this.data('sparkline-zerobar-color') || '#909090', thisTristateBarWidth = $this.data('sparkline-barwidth') || 5, thisTristateBarSpacing = $this.data('sparkline-barspacing') || 2, thisZeroAxis = $this.data('sparkline-zeroaxis') || false;

                        $this.sparkline('html', {
                            type: 'tristate',
                            height: thisTristateHeight,
                            posBarColor: thisBarColor,
                            negBarColor: thisTristateNegBarColor,
                            zeroBarColor: thisTristateZeroBarColor,
                            barWidth: thisTristateBarWidth,
                            barSpacing: thisTristateBarSpacing,
                            zeroAxis: thisZeroAxis

                        });

                    }

                    //COMPOSITE: BAR
                    if (sparklineType == 'compositebar') {

                        var sparklineHeight = $this.data('sparkline-height') || '20px', sparklineWidth = $this.data('sparkline-width') || '100%', sparklineBarWidth = $this.data('sparkline-barwidth') || 3, thisLineWidth = $this.data('sparkline-line-width') || 1, thisLineColor = $this.data('sparkline-color-top') || '#ed1c24', thisBarColor = $this.data('sparkline-color-bottom') || '#333333';

                        $this.sparkline($this.data('sparkline-bar-val'), {
                            type: 'bar',
                            width: sparklineWidth,
                            height: sparklineHeight,
                            barColor: thisBarColor,
                            barWidth: sparklineBarWidth
                            //barSpacing: 5

                        });

                        $this.sparkline($this.data('sparkline-line-val'), {
                            width: sparklineWidth,
                            height: sparklineHeight,
                            lineColor: thisLineColor,
                            lineWidth: thisLineWidth,
                            composite: true,
                            fillColor: false

                        });

                    }

                    //COMPOSITE: LINE
                    if (sparklineType == 'compositeline') {

                        var sparklineHeight = $this.data('sparkline-height') || '20px', sparklineWidth = $this.data('sparkline-width') || '90px', sparklineValue = $this.data('sparkline-bar-val'), sparklineValueSpots1 = $this.data('sparkline-bar-val-spots-top') || null, sparklineValueSpots2 = $this.data('sparkline-bar-val-spots-bottom') || null, thisLineWidth1 = $this.data('sparkline-line-width-top') || 1, thisLineWidth2 = $this.data('sparkline-line-width-bottom') || 1, thisLineColor1 = $this.data('sparkline-color-top') || '#333333', thisLineColor2 = $this.data('sparkline-color-bottom') || '#ed1c24', thisSpotRadius1 = $this.data('sparkline-spotradius-top') || 1.5, thisSpotRadius2 = $this.data('sparkline-spotradius-bottom') || thisSpotRadius1, thisSpotColor = $this.data('sparkline-spot-color') || '#f08000', thisMinSpotColor1 = $this.data('sparkline-minspot-color-top') || '#ed1c24', thisMaxSpotColor1 = $this.data('sparkline-maxspot-color-top') || '#f08000', thisMinSpotColor2 = $this.data('sparkline-minspot-color-bottom') || thisMinSpotColor1, thisMaxSpotColor2 = $this.data('sparkline-maxspot-color-bottom') || thisMaxSpotColor1, thishighlightSpotColor1 = $this.data('sparkline-highlightspot-color-top') || '#50f050', thisHighlightLineColor1 = $this.data('sparkline-highlightline-color-top') || '#f02020', thishighlightSpotColor2 = $this.data('sparkline-highlightspot-color-bottom') || thishighlightSpotColor1, thisHighlightLineColor2 = $this.data('sparkline-highlightline-color-bottom') || thisHighlightLineColor1, thisFillColor1 = $this.data('sparkline-fillcolor-top') || 'transparent', thisFillColor2 = $this.data('sparkline-fillcolor-bottom') || 'transparent';

                        $this.sparkline(sparklineValue, {
                            type: 'line',
                            spotRadius: thisSpotRadius1,

                            spotColor: thisSpotColor,
                            minSpotColor: thisMinSpotColor1,
                            maxSpotColor: thisMaxSpotColor1,
                            highlightSpotColor: thishighlightSpotColor1,
                            highlightLineColor: thisHighlightLineColor1,

                            valueSpots: sparklineValueSpots1,

                            lineWidth: thisLineWidth1,
                            width: sparklineWidth,
                            height: sparklineHeight,
                            lineColor: thisLineColor1,
                            fillColor: thisFillColor1

                        });

                        $this.sparkline($this.data('sparkline-line-val'), {
                            type: 'line',
                            spotRadius: thisSpotRadius2,

                            spotColor: thisSpotColor,
                            minSpotColor: thisMinSpotColor2,
                            maxSpotColor: thisMaxSpotColor2,
                            highlightSpotColor: thishighlightSpotColor2,
                            highlightLineColor: thisHighlightLineColor2,

                            valueSpots: sparklineValueSpots2,

                            lineWidth: thisLineWidth2,
                            width: sparklineWidth,
                            height: sparklineHeight,
                            lineColor: thisLineColor2,
                            composite: true,
                            fillColor: thisFillColor2

                        });

                    }
                }


            }
        };
    }])
    .directive("easyPieChart", [function () {
        return {
            restrict: 'E',
            replace: true,
            transclude: false,
            compile: function (element, attrs) {
                var html = "<div class='easy-pie-chart " + attrs.color + "' data-percent='" + attrs.percent + "' data-pie-size='" + attrs.size + "'><span class='percent percent-sign'>" + attrs.value + "</span></div><span class='easy-pie-title'>" + attrs.title + "</span>";
                var newElem = angular.element(html);
                element.replaceWith(newElem);
                return function (scope, elem) {
                    var $this = elem;
                    var barColor = $this.css('color') || $this.data('pie-color'), trackColor = $this.data('pie-track-color') || '#eeeeee', size = parseInt($this.data('pie-size')) || 25;
                    $this.easyPieChart({
                        barColor: barColor,
                        trackColor: trackColor,
                        scaleColor: false,
                        lineCap: 'butt',
                        lineWidth: parseInt(size / 8.5),
                        animate: 1500,
                        rotate: -90,
                        size: size,
                        onStep: function (value) {
                            this.$el.find('span').text(~~value);
                        }
                    });
                };

            }
        };
    }])
    .directive('noisFocus', function () {
        return function (scope, element) {
            setTimeout(function () { element[0].focus(); }, 200);
        };
    })
    .directive("datepicker", ['$timeout', function ($timeout) {
        return {
            restrict: "A",
            require: 'ngModel',
            link: function (scope, element, attrs, ngModelCtrl) {

                $timeout(function () {
                    var dataDateFormat = attrs.format || 'mm-dd-yy';
                    var defDate = attrs.defaultDate;
                    var minYear = angular.isDefined(attrs.minYear) ? attrs.minYear : 1910;
                    var maxYear = angular.isDefined(attrs.maxYear) ? attrs.maxYear : new Date().getFullYear();

                    if (ngModelCtrl.$viewValue.toString() !== 'NaN' && ngModelCtrl.$viewValue !== '') {
                        defDate = ngModelCtrl.$viewValue;
                    }

                    var limittype = attrs.limitType;
                    console.log($(function () {
                        var datepickerObject = jQuery("#" + attrs.limitid);

                        element.keyup(function () {
                            if (jQuery(this).val() === '') {
                                if (limittype === "mindate") {
                                    datepickerObject.datepicker("option", "minDate", null);
                                } else if (limittype === "maxdate") {
                                    datepickerObject.datepicker("option", "maxDate", null);
                                }
                            }
                        });

                        element.datepicker({
                            defaultDate: defDate,
                            dateFormat: dataDateFormat,
                            nextText: '<i class="fa fa-chevron-right"></i>',
                            prevText: '<i class="fa fa-chevron-left"></i>',
                            changeMonth: true,
                            changeYear: true,
                            yearRange: minYear + ':' + maxYear,
                            onSelect: function (date) {
                                if (limittype === "mindate") {
                                    datepickerObject.datepicker("option", "minDate", date);
                                } else if (limittype === "maxdate") {
                                    datepickerObject.datepicker("option", "maxDate", date);
                                }
                                scope.$apply(function () {
                                    ngModelCtrl.$setViewValue(date);
                                });
                            }
                        });

                    }));
                });

            }
        };
    }])
    .directive('pwCheck', [function () {
        return {
            require: 'ngModel', link: function (scope, elem, attrs, ctrl) {
                var firstPassword = '#' + attrs.pwCheck; elem.add(firstPassword).on('keyup', function () {
                    scope.$apply(function () {
                        var v = elem.val() === angular.element(firstPassword).val();
                        ctrl.$setValidity('pwmatch', v);
                    });
                });
            }
        };
    }])
    .directive("autoFillSync", ['$interval', function ($interval) {
        return {
            restrict: "A",
            require: 'ngModel',
            link: function (scope, elem, attrs, ngModel) {
                $interval(function () {
                    ngModel.$setViewValue(elem.val());
                }, 500);
                elem.focus(function () {
                    ngModel.$setViewValue(elem.val());
                });
            }
        };
    }])
    .directive("indicator", ['helperSrv', '$compile', function (helperSrv, $compile) {
        return {
            restrict: "A",
            scope: {
                show: '=',
                indicator: '='
            },
            replace: true,
            link: function (scope, elem, attrs) {

                var type = scope.indicator;

                var getClasses = function () {
                    if (type == 0) {
                        return 'color:#2C3742;';
                    }
                    else if (type == 1) {
                        return 'color:#2C3742; line-height: 21px; margin: 3px 0px 0px 5px; position: absolute; top: 10px; left: 48%; z-index: 0;';
                    }
                    else if (type == 2) {
                        return 'float:right; margin:13px 0 0 5px; color:#2C3742; font-size:26px;';
                    }
                    else if (type == 3) {
                        return 'float: right; margin: 4px 7px 0 5px; color: #2C3742';
                    }
                    else if (type == 4) {
                        return 'color: #2C3742; position: absolute; top: 35%; left: 48%;';
                    }
                    else if (type == 5) {
                        return 'color: #2C3742; margin-left: 5px; position: relative; top: 5px; line-height: 20px;';
                    }
                    else if (type == 6) {
                        return 'color: #2C3742; margin: 3px 0 0 5px; line-height: 21px;';
                    }
                    else if (type == 7) {
                        return 'color: #fff; margin: 0 10px 0 0;';
                    }
                    else if (type == 8) {
                        return 'color: #2C3742; position: absolute; top: 20%; left: 48%;';
                    }
                    else if (type == 9) {
                        return 'color: #2C3742; float: left; margin: 3px 0px 0px 5px;';
                    }
                    return "";
                }

                var template = '<i class="custom-spinning ' + attrs.class + '" style="' + getClasses() + '" ng-show="show"></i>';
                if (helperSrv.isIe() === 9) {
                    template = '<i class="ie-indicator" style="' + getClasses() + '" ng-show="show"><img src="Content/themes/smart/img/ie-preloader.GIF"  alt="loading"/></i>';
                }

                var newElement = angular.element("<span />").html(template);
                elem.replaceWith(newElement);
                $compile(newElement.contents())(scope);
            }
        };
    }])
    .directive("indicatorShown", [function () {
        function isIe() {
            var myNav = navigator.userAgent.toLowerCase();
            return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
        }

        var template = '<i ng-class="addClass" style="{{attrs.style}}"></i>';
        if (isIe() === 9) {
            template = '<i class="ie-indicator" style="{{attrs.style}}"><img src="Content/themes/smart/img/ie-preloader.GIF"  alt="loading"/></i>';
        }
        return {
            restrict: "E",
            replace: true,
            link: function (scope, elem, attrs) {
                scope.addClass = "fa fa-spinner fa-spin " + attrs.class;
            },
            template: template
        };
    }])
    .directive('ngVisible', function () {
        return function (scope, element, attr) {
            scope.$watch(attr.ngVisible, function (visible) {
                element.css('visibility', visible ? 'visible' : 'hidden');
            });
        };
    })
    .directive("indicatorVisible", [function () {
        function isIe() {
            var myNav = navigator.userAgent.toLowerCase();
            return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
        }

        var template = '<i ng-class="addClass" style="{{attrs.style}}" ng-visible="show"></i>';
        if (isIe() === 9) {
            template = '<i class="ie-indicator" style="{{attrs.style}}" ng-visible="show"><img src="Content/themes/smart/img/ie-preloader.GIF"  alt="loading"/></i>';
        }
        return {
            restrict: "E",
            scope: {
                show: '='
            },
            replace: true,
            link: function (scope, elem, attrs) {
                scope.addClass = "fa fa-spinner fa-spin " + attrs.class;
            },
            template: template
        };
    }])
    .directive("fadeIn", function () {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {
                jQuery(elm)
                    .css({ opacity: 0 })
                    .animate({ opacity: 1 }, parseInt(attrs.fadeIn));
            }
        };
    })
    .directive('blink', ['$timeout', function ($timeout) {
        return {
            restrict: 'A',
            scope: {
                blink: '='
            },
            controller: ['$scope', '$element', function ($scope, $element) {
                var attrClass = "blinking";
                showElement();

                function showElement() {
                    if ($scope.blink) {
                        $element.addClass(attrClass);
                    } else {
                        $element.removeClass(attrClass);
                    }
                    $timeout(function () {
                        hideElement();
                    }, 1000);
                }

                function hideElement() {
                    $element.removeClass(attrClass);
                    $timeout(function () {
                        showElement();
                    }, 1000);
                }
            }]
        };
    }])
    .directive('textareamaxlengthwithbr', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var maxLength = attrs.textareamaxlengthwithbr;
                elem.on('keyup', function () {
                    var inputText = elem.val().replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, "<br/>");
                    ctrl.$setValidity('maxLengthValid', inputText.length <= maxLength - 10);
                    //scope.$apply(function () {
                    //    //var v = elem.val() === $(firstPassword).val();
                    //    
                    //});
                });
            }
        };
    }])
    .directive('paginate', ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                pages: '@paginatePages',
                currentPage: '=',
                control: '='
            },
            template: '<ul class="pagination">' + '</ul>',
            controller: ['$scope', '$element', function ($scope, $element) {
                var halfDisplayed = 1.5,
                    displayedPages = 3,
                    edges = 2;

                $scope.getInterval = function () {
                    return {
                        start: Math.ceil($scope.currentPage > halfDisplayed ? Math.max(Math.min($scope.currentPage - halfDisplayed, ($scope.pages - displayedPages)), 0) : 0),
                        end: Math.ceil($scope.currentPage > halfDisplayed ? Math.min($scope.currentPage + halfDisplayed, $scope.pages) : Math.min(displayedPages, $scope.pages))
                    };
                };

                $scope.selectPage = function (pageIndex) {
                    $scope.currentPage = pageIndex;
                    $scope.draw();
                    $scope.$apply();
                };

                $scope.appendItem = function (pageIndex, opts) {
                    var options, link;

                    pageIndex = pageIndex < 0 ? 0 : (pageIndex < $scope.pages ? pageIndex : $scope.pages - 1);

                    options = $.extend({
                        text: pageIndex + 1,
                        classes: '',
                        disabled: false
                    }, opts || {});

                    if (pageIndex == $scope.currentPage) {
                        if (options.disabled) {
                            link = angular.element('<li><a href="javascript:void(0)">' + (options.text) + '</a></li>');
                        } else {
                            link = angular.element('<li class="active"><a href="javascript:void(0)">' + (options.text) + '</a></li>');
                        }

                    } else {
                        link = angular.element('<li><a href="javascript:void(0)">' + (options.text) + '</a></li>');
                        link.find("a").bind('click', function () {
                            $scope.selectPage(pageIndex);
                        });
                    }

                    if (options.classes) {
                        link.addClass(options.classes);
                    }

                    $element.append(link);
                };
                $scope.draw = function () {
                    angular.element($element).empty();
                    if ($scope.pages <= 1)
                    { return; }
                    var interval = $scope.getInterval(),
                        i;

                    // Generate Prev link
                    if (true) {
                        var previousClass = 'prev';
                        if ($scope.currentPage === 0) {
                            previousClass += ' disabled';
                        }
                        $scope.appendItem($scope.currentPage - 1, {
                            text: 'Previous',
                            classes: previousClass,
                            disabled: true
                        });
                    }

                    // Generate start edges
                    if (interval.start > 0 && edges > 0) {
                        var end = Math.min(edges, interval.start);
                        for (i = 0; i < end; i++) {
                            $scope.appendItem(i);
                        }
                        if (edges < interval.start) {
                            $element.append('<li class="ellipse"><span>...</span></li>');
                        }
                    }

                    // Generate interval links
                    for (i = interval.start; i < interval.end; i++) {
                        $scope.appendItem(i);
                    }

                    // Generate end edges
                    if (interval.end < $scope.pages && edges > 0) {
                        if ($scope.pages - edges > interval.end) {
                            $element.append('<li class="ellipse"><span>...</span></li>');
                        }
                        var begin = Math.max($scope.pages - edges, interval.end);
                        for (i = begin; i < $scope.pages; i++) {
                            $scope.appendItem(i);
                        }
                    }

                    // Generate Next link
                    if (true) {
                        var nextClass = 'next';
                        if ($scope.currentPage === $scope.pages - 1) {
                            nextClass += ' disabled';
                        }
                        $scope.appendItem($scope.currentPage + 1, {
                            text: 'Next',
                            classes: nextClass,
                            disabled: true
                        });
                    }
                };
            }],
            link: function (scope, element, attrs) {
                scope.internalControl = scope.control || {};
                scope.internalControl.reDraw = function (pI) {
                    scope.currentPage = pI;
                    scope.draw();
                };

                $timeout(function () {
                    scope.draw();
                }, 500);

                attrs.$observe("currentPage", function () {
                    scope.draw();
                });
            }
        };
    }])
    .directive('browserVersion', ['helperSrv', function (helperSrv) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                if (helperSrv.isIe() === 9) {
                    // add class to html
                    jQuery("html").addClass("notsupportdragdrop");
                }

                // Release Mode
                if ('ontouchstart' in document.documentElement) {
                    jQuery("html").addClass("on-device");
                }

                // Debug Mode
                //jQuery("html").addClass("on-device");
            }
        }
    }])
    .directive('specialPage', ['$location', '$timeout', function ($location, $timeout) {
        return {
            restrict: "A",
            link: function (scope) {
                var path = function () { return $location.path(); };

                scope.$watch(path, function (newVal, oldVal) {
                    addClass();
                });

                var addClass = function () {
                    if (path() == '/signin') {
                        angular.element("#header .pull-right").hide();
                        angular.element("#left-panel").hide();
                        if (!angular.element("body").hasClass("special-page"))
                            angular.element("body").addClass("special-page");
                    } else {
                        angular.element("#header .pull-right").show();
                        angular.element("#left-panel").show();
                        if (angular.element("body").hasClass("special-page"))
                            angular.element("body").removeClass("special-page");
                    }
                }

                addClass();
            }
        }
    }])
    .directive('activePageSidebar', ['ApplicationFactory', '$state', '$rootScope', 'TopicFactory', 'TaskFactory', function (applicationFactory, $state, $rootScope, topicFactory, taskFactory) {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var pagename = attrs.activePageSidebar;
                $rootScope.$on('$stateChangeStart',
                    function (event, toState, toParams, fromState, fromParams) {
                        var sysPageName = toState.name;
                        activeSidebarMenu(sysPageName);
                    });
                var listener = scope.$watch(function () { return $state.current.name; }, function () {
                    activeSidebarMenu($state.current.name);
                }, true);

                function activeSidebarMenu(sysPageName) {

                    if (sysPageName === 'default' || sysPageName === '') {
                        sysPageName = 'Home';
                    }
                    if (angular.lowercase(sysPageName) === angular.lowercase(pagename)) {
                        elem.addClass("active");
                    } else {
                        elem.removeClass("active");
                    }
                }

                elem.bind('click', function () {
                    if ((angular.lowercase(pagename) == "default" || angular.lowercase(pagename) == "home") && ($state.current.name === 'home' || $state.current.name === 'default')) {
                        // reload topics in topic page (home page)
                        topicFactory.reloadTopics();
                    }
                    else if (angular.lowercase(pagename) == "task" && $state.current.name === 'task') {
                        // reload tasks in task page
                        taskFactory.reloadTasks();
                    }
                });

                scope.$on('$destroy', function () {
                    listener();
                    elem.unbind('click');
                });
            }
        }
    }])
    .directive('uploadControl', ['helperSrv', 'globalConfig', 'myAlert', 'ApplicationFactory', '$timeout', '$http', function (helperSrv, globalConfig, myAlert, applicationFactory, $timeout, $http) {
        return {
            restrict: "A",
            scope: {
                uploadControlAttachmentsList: '=',
                filesList: '=',
                defer: '=',
                iframe: '=',
                uploadUrl: '=',
                isClear: '='
            },
            link: function (scope, elem, attrs) {

                var fileSize = globalConfig.fileSize;
                var currentFileIndex = 0;
                var submitForm;
                var autoUpload = attrs.autoUpload || false;

                scope.$watch("defer", function () {
                    autoUpload = attrs.autoUpload || false;
                });

                // add file to array list
                var addAttachmentToList = function (dt) {
                    if (!autoUpload) {
                        if (!helperSrv.supportDragDrop()) {
                            scope.uploadControlAttachmentsList.push({ name: dt });
                        }
                        else {
                            scope.uploadControlAttachmentsList.push({ name: dt.name });
                        }
                    }
                }

                // get filename from path
                var getFileNameFromPath = function (fullPath) {
                    var index = fullPath.lastIndexOf("\\") + 1;
                    if (index === 0) {
                        index = fullPath.lastIndexOf("/") + 1;
                    }
                    var filename = fullPath.substr(index);

                    return filename;
                };

                // validate file function
                var validateFile = function (fileName, filesi) {
                    var validFile = true;

                    //validate extension
                    if (!(globalConfig.regexAllowedExtension).test(fileName)) {
                        // show alert box
                        myAlert.pop('e', 'Error', 'Only accepted file types: ' + globalConfig.allowedExtension);
                        validFile = false;
                    }

                    //validate fileSize (not support in <= IE 9)
                    if (filesi > 0 && filesi > fileSize) { // 2mb
                        // show alert box
                        myAlert.pop('e', 'Error', 'Please upload a smaller file, max size is 0.5 MB (512kb)');

                        validFile = false;
                    }

                    //validate maxFiles
                    if (!autoUpload) {
                        if (scope.uploadControlAttachmentsList.length >= 3) {
                            // show alert box
                            myAlert.pop('e', 'Error', 'Max files is reached');

                            validFile = false;

                        }
                    }
                    return validFile;
                }

                // change file upload control
                var changeFileUploadControl = function (e) {
                    if (typeof jQuery(e) != 'undefined') {
                        // remove active state at upload control
                        jQuery(e).removeClass("active-file");

                        // add has file state to upload control
                        jQuery(e).addClass("has-file");

                        // Go to next upload control
                        if (typeof jQuery(e).next() != 'undefined') {
                            if (!jQuery(e).next().hasClass("has-file")) {
                                jQuery(e).next().addClass("active-file");
                            } else {
                                // change upload control
                                changeFileUploadControl(jQuery(e).next());
                            }
                        }
                    }
                }

                // file change function
                var fileChange = function () {
                    // get file name
                    var fileName = getFileNameFromPath(jQuery(this).val());

                    // remove file error state
                    jQuery(this).removeClass("file-error");

                    // validate file
                    if (!validateFile(fileName, 0)) {
                        // add error state
                        jQuery(this).addClass("file-error");
                    } else {
                        if (autoUpload) {
                            // auto submit form
                            submitForm();
                        } else {
                            // add file to array list
                            addAttachmentToList(fileName);

                            // change to another upload control
                            changeFileUploadControl(jQuery(this));
                        }

                    }
                }

                // create upload control (Fix IE)
                var createUploadControl = function () {
                    //clear old file upload control
                    elem.find("#file-upload-control").html('');

                    //append new file upload control
                    for (var i = 0; i < globalConfig.maxFiles; i++) {
                        jQuery("<input type='file' id='file-" + i + "' class='ie-file " + (i == 0 ? "active-file" : "") + "' name='files[]'>").appendTo(elem.find("#file-upload-control")).bind("change", fileChange);
                        currentFileIndex++;
                    }
                }

                // check if are there any file upload control
                if (elem.find("#file-upload-control input[type=file]").size() > 0) {
                    return;
                }

                elem.on("click", ".remove-attachment", function (e) {
                    e.preventDefault();
                    var idx = parseInt(angular.element(this).data("index"));
                    // remove file from reply scope
                    scope.uploadControlAttachmentsList.splice(idx, 1);

                    //remove file from fileList array
                    scope.filesList.splice(idx, 1);

                    // remove file upload and replace new one
                    if (!helperSrv.supportDragDrop()) {
                        // remove input file
                        elem.parents('form').find("#file-upload-control").children("input").eq(idx).remove();

                        var clss = 'ie-file';
                        if (!elem.parents('form').find("#file-upload-control").find(".active-file").length > 0) {
                            clss += ' active-file';
                        }

                        // add new input file
                        jQuery("<input type='file' id='file-" + currentFileIndex++ + "' class='" + clss + "' name='files[]'>").appendTo(elem.parents('form').find("#file-upload-control")).bind("change", fileChange);
                    }
                });

                //create unique id
                var frameId = "upload_frame_" + new Date().getTime();

                //Check if browser support drag and drop file

                if (!helperSrv.supportDragDrop()) {
                    // create upload controls
                    createUploadControl();

                    //append iframe for post
                    elem.parents("form").append('<iframe id="' + frameId + '" name="' + frameId + '" class="hidden"></iframe>');

                    // handle event when iframe loaded
                    angular.element('#' + frameId).on('load', function () {
                        //parse result to json
                        var resp = angular.fromJson(jQuery(this).contents().find('pre').html());
                        if (angular.isDefined(resp)) {

                            // resolve the promise
                            scope.defer.resolve(resp);

                            // reset file upload control
                            createUploadControl();
                        }
                    });

                    $timeout(function () {
                        submitForm = function () {
                            //get form parent
                            var $form = elem.parents("form");

                            //remove input file without selected file
                            $form.find("#file-upload-control input[type=file]").each(function () {
                                if (!jQuery(this).hasClass("has-file")) {
                                    jQuery(this).remove();
                                }
                            });

                            //add attribute to form
                            $form.attr('action', scope.uploadUrl);

                            $form.attr('method', 'post');
                            $form.attr('enctype', 'multipart/form-data');

                            //change target to iframe and submit
                            $form.attr('target', frameId).submit();
                        }

                        // bind submit to scope
                        scope.iframe.submit = submitForm;
                    }, 100);

                    scope.$watch("isClear", function () {
                        if (scope.isClear) {
                            // reset file upload control
                            createUploadControl();
                        }
                    });

                } else {
                    // create upload method
                    submitForm = function () {
                        $http({
                            method: 'POST',
                            url: scope.uploadUrl,
                            data: scope.filesList,
                            cache: false,
                            headers: { 'Content-Type': undefined },
                            transformRequest: function (data) {
                                if (typeof FormData != 'undefined') {
                                    var fd = new FormData();
                                    angular.forEach(data, function (value, key) {
                                        fd.append(key, value);
                                    });
                                    return fd;
                                }
                            }
                        }).success(function (resp) {
                            // resolve the promise
                            scope.defer.resolve(resp);
                        });
                    }

                    //add dropzone
                    elem.find("#file-upload-control").append("<input type='file' name='files[]' multiple>");

                    //file upload config
                    var config = {
                        dropZone: elem.find('.fu-dropzone'),
                        autoUpload: false
                    };

                    //setup file upload
                    elem
                        .find("#file-upload-control input[type=file]")
                        .fileupload(config)
                        .bind('fileuploadadd', function (e, data) {
                            // Turn the FileList object into an Array
                            for (var i = 0; i < data.files.length; i++) {
                                //if file is valiad
                                if (validateFile(data.files[i].name, data.files[i].size)) {

                                    // add file to array list
                                    addAttachmentToList(data.files[i]);

                                    // add selected file to array list
                                    scope.filesList.push(data.files[i]);

                                    if (autoUpload) {
                                        // auto submit file
                                        submitForm();
                                    }
                                }
                            }
                        })
                        .bind('fileuploaddone', function (e, data) {
                            // process the form
                            //processTheForm(data.jqXHR.responseJSON);
                        })
                        .bind('fileuploadprogressall', function () { });
                }

                // on scope destroy
                scope.$on('$destroy', function () {
                    // remove the iframe when scope is destroyed
                    angular.element("#" + frameId).off("load");
                    angular.element("#" + frameId).remove();
                    elem.off("click");
                });
            }
        }
    }])
    .directive('userStats', ['ApplicationFactory', 'TaskFactory', function (applicationFactory, taskFactory) {
        return {
            restrict: 'A',
            scioe: true,
            link: function (scope) {
                scope.taskCompleted = taskFactory.statistic.taskCompleted;
                scope.taskToDo = taskFactory.statistic.taskToDo;
                scope.userData = applicationFactory.userData;
            }
        }
    }])
    .directive('hideMenu960', ['$window', '$location', function ($window, $location) {
        return {
            restrict: "A",
            link: function (scope) {

                var path = function () { return $location.path(); };

                scope.$watch(path, function (newVal, oldVal) {
                    hideSidebar();
                });

                var hideSidebar = function () {
                    if (angular.element($window).width() <= 960) {
                        angular.element('body').removeClass("hidden-menu");
                    }
                }
            }
        }
    }])
    .directive('hideUntilReady', ['$timeout', '$rootScope', function ($timeout, $rootScope) {
        return {
            restrict: "A",
            link: function (scope, elem) {
                $rootScope.$on('$stateChangeSuccess', function () {
                    elem.removeClass("hidden");
                });
            }
        }
    }])
    .directive('multipleSelect', [function () {
        return {
            restrict: "A",
            require: 'ngModel',
            scope: {
                data: '=multipleSelect'
            },
            link: function (scope, elem, attrs, ctrl) {
                var nonSelectedText = attrs.nonSelectedText || "Select below";
                // watch values change
                scope.$watch("data", function () {
                    // initial multi select
                    elem.multiselect({
                        noneSelectedText: nonSelectedText
                    });

                    // clear options
                    elem.find('option').remove();

                    // Append new options
                    for (var i = 0; i < scope.data.length; i++) {
                        elem.append('<option value="' + scope.data[i] + '">' + scope.data[i] + '</option>');
                    }

                    // refresh multi select 
                    elem.multiselect('refresh');
                });

                // handle choose item
                elem.on("multiselectclick", function (event, ui) {
                    /* event: the original event object ui.value: value of the checkbox ui.text: text of the checkbox ui.checked: whether or not the input was checked or unchecked (boolean) */
                    ctrl.$setViewValue(elem.val());
                });
            }
        }
    }])
    .directive('sidebarMenu', [function () {
        return {
            restrict: 'A',
            scope: {
                sidebarMenu: '@'
            },
            template: '<ul><li ng-repeat="menu in menus" data-active-page-sidebar="{{ menu.name }}"><a href="{{menu.link}}" title="{{menu.name}}"><i class="fa fa-lg fa-fw" ng-class="\'fa-{{menu.name}}\'"></i><span class="menu-item-parent">{{menu.name}}</span></a></li></ul>',
            controller: ['$scope','$state',function ($scope, $state) {
                $scope.$watch('sidebarMenu', function () {
                    var menus = angular.fromJson($scope.sidebarMenu);
                    $scope.menus = menus;
                    $scope.menus.forEach(function (menu) {
                        // ui-router didn't support dynamic binding to ui-sref directive (https://github.com/angular-ui/ui-router/issues/395)
                        // We're going to use $state.href(statename) to generate url based on the statename that was configured on the router.js
                        menu.link = $state.href(menu.url);
                    });
                });
            }]
        }
    }])


.directive("showHideMainContent", ['$timeout', '$rootScope', function ($timeout, $rootScope) {
    return {
        restrict: 'A',
        link: function (scope) {
            scope.$on('SHOW-HIDE-MENU', function (event, params) {
                var isShow = params.isShow;
                if (isShow) {
                    angular.element(".fadeWhenChange").stop(true).fadeOut(300, function () {
                        angular.element(".pre-loading").stop(true).fadeIn(300, function () {
                        });
                    });
                } else {
                    $timeout(function () {
                        angular.element(".pre-loading").hide();
                        angular.element(".fadeWhenChange").fadeIn();
                    }, 500);
                }
            });
            scope.$on('SHOW-HIDDEN-MENU', function () {
                angular.element("body").removeClass("hidden-menu");
            })
        }
    }
}])