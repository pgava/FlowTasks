flowTasksApp
    // Topic list
    .directive("topicList", [function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'scripts/flowTasksApp/views/topic/index.html'
        };
    }])

    // select topic search
    .directive("selectTopicSearch", [function () {
        return {
            restrict: "A",
            scope: {
                chooseTopic: '&selectTopicSearch',
                item: '=searchItem'
            },
            link: function (scope, elem, attrs) {
                elem.on("click", function () {
                    elem.addClass("selected");

                    // do action
                    scope.chooseTopic({ sitem: scope.item });
                });

                scope.$on("$destroy", function () {
                    elem.off("click");
                });
            }
        }
    }])

    // Topic box in list
    .directive("topicBox", ['TopicSrv', '$timeout', 'globalConfig', 'helperSrv', '$document', '$window', '$interval', 'WorkContextSrv', '$rootScope', 'ApplicationFactory', '$q', 'myAlert', 'TopicFactory', function (topicSrv, $timeout, globalConfig, helperSrv, $document, $window, $interval, workContextSrv, $rootScope, applicationFactory, $q, myAlert, topicFactory) {
        return {
            scope: {
                topic: '=',
                showdate: '&'
            },
            replace: true,
            restrict: 'E',
            templateUrl: 'scripts/flowTasksApp/views/topic/box.html',
            link: function (scope, element) {
                var username = applicationFactory.userData.userName;                                // Get username
                var photoPath = applicationFactory.userData.photoPath;                              // Get user photo
                var maxFlood = 5;                                                                   // Default Flood time
                var isOkToPostReply = false;                                                        // The value indicating whether can post reply
                var floodTime = null;                                                               // Flood timeout object
                var maxLength = 500;                                                                // Textarea maxlength

                // fn validate form
                var validate = function (input, type) {
                    if (type === 'required') {
                        return (scope.showError && (typeof input === 'undefined' || input === ''));
                    }
                    else if (type === 'maxlength') {
                        if (typeof input !== 'undefined') {
                            var ipt = input.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, "<br/>");
                            return ipt.length > maxLength - 10;
                        }
                    }
                    return false;
                }

                // fn scroll to bottom
                var scrollToBottom = function () {
                    var elem2 = jQuery(element).find("#chat-body");
                    if (typeof elem2[0] != 'undefined') {
                        var height2 = elem2[0].scrollHeight;
                        elem2.scrollTop(height2);
                    }
                }

                // fn add reply
                var addReply = function (resp) {
                    if (resp.result) {

                        //get attachments
                        var attachmentList = [];
                        if (resp.reply.attachments != null) {
                            for (var i = 0; i < resp.reply.attachments.length; i++) {
                                attachmentList.push({ fileName: resp.reply.attachments[i].fileName, oid: resp.reply.attachments[i].documentOid });
                            }
                        }

                        //remove file list
                        scope.filesList = [];

                        //reset model
                        //append reply to replies list
                        scope.topicReplies.recentReplies.push({ "status": "New", "imageUrl": photoPath, "from": username, "to": "", "when": "", "message": resp.reply.message, "attachments": attachmentList });

                        //reset model
                        scope.reply = { body: '', entertosend: scope.reply.entertosend, attachments: [] };

                        //scroll window to bottom
                        scrollToBottom();
                    } else {
                        //show error
                        scope.hasErrorWhilePosting = true;
                        scope.errorMessage = resp.errorMessage;
                    }

                    scope.isPosting = false;
                    //Re-counting
                    scope.replyFlood = maxFlood;
                    //create counting
                    floodTime = $interval(function () {
                        if (scope.replyFlood == 0) {
                            $interval.cancel(floodTime);
                            floodTime = null;
                            scope.floodError = false;
                        } else {
                            scope.replyFlood = scope.replyFlood - 1;
                        }
                    }, 1000);
                }

                // handle topic box collapse event
                var topicBoxCollapsed = function (p) {
                    if (!p) {
                        scope.topicReply.templateUrl = '';
                        // hide reply box
                        angular.element(element).find(".topic-replies").removeClass("slidein");
                    } else {
                        // show reply box
                        angular.element(element).find(".topic-replies").addClass("slidein");
                    }
                    scope.topicReplies.hasExpanded = p;
                };

                // collapse if click outside topic box
                $document.bind('click', function (e) {
                    if (!angular.element(e.target).parents(".topic-message").length > 0 &&
                        !angular.element(e.target).parents(".topic-replies").length > 0 &&
                        !angular.element(e.target).hasClass('remove-attachment') &&
                        !angular.element(e.target).hasClass('topic-box-loading-replies') &&
                        !angular.element(e.target).hasClass('modal') &&
                        !angular.element(e.target).parents(".modal").length > 0) {

                        // clear reploy box template url
                        //scope.topicReply.templateUrl = '';

                        // close reply box
                        topicBoxCollapsed(false);
                    }
                });

                // scroll to bottom
                scrollToBottom();

                // scope variables
                scope.topic.showDateDivider = scope.showdate({ myDate: scope.topic.message.whenDay });

                scope.allowedExtension = globalConfig.allowedExtension;                             // Allowed attachment extensions

                scope.maximumFiles = globalConfig.maxFiles;                                         // Maximum files allowed to be uploaded

                /* Start Setup submit reply */
                scope.replyFlood = maxFlood;                                                        // Flood time
                scope.reply = { body: '', entertosend: false, attachments: [] };                    // Reply view model
                scope.showError = false;                                                            // Show error state
                scope.floodError = false;                                                           // Show flood error state
                scope.isPosting = false;                                                            // Is posting reply state (Submiting)
                scope.hasErrorWhilePosting = false;                                                 // Has error while posting reply state
                scope.errorMessage = '';                                                            // Error message
                scope.filesList = [];                                                               // Files list
                scope.defer = null;                                                                 // Promise
                scope.postUrl = topicFactory.replyUrl(scope.topic.topicId, username, scope.topic.message.to);
                scope.iframe = {};                                                                  // Contains functions was created from Iframe
                scope.topicReply = {
                    templateUrl: ''                                                                 // Reply box template
                };

                //replies
                scope.topicReplies = {
                    recentReplies: [],                                                              // Recent replies array
                    olderReplies: [],                                                               // Old replies array
                    loadingReplies: false,                                                          // On loading replies state
                    loadingOlderReplies: false,                                                     // On loading old replies state
                    showOlderReplies: false,                                                        // Show old replies state
                    hasOlderReplies: false,                                                         // Has old replies state
                    hasExpanded: false                                                              // Topic replies has been expanded state
                };

                //validate input
                scope.validate = function (field, type) {
                    return validate(scope.reply.body, type);
                };

                //submit reply
                scope.submitReply = function (reply, topic, frombutton) {
                    if (scope.isPosting) {
                        return;
                    }

                    scope.hasErrorWhilePosting = false;
                    isOkToPostReply = false;

                    scope.showError = true;

                    //if the body is empty then show error
                    if ((validate(reply.body, 'required') || validate(reply.body, 'maxlength'))) {
                        return;
                    }

                    //if topic and reply is not undefined and null
                    if (typeof topic != 'undefined' && topic != null) {
                        //if body is empty
                        if (reply.body === '') {
                            return;
                        }
                        //hide error
                        scope.showError = false;

                        //if user click on button
                        if (frombutton) {
                            isOkToPostReply = true;
                        } else {
                            //if 'Enter to send' is true
                            var enterToSend = (typeof reply.entertosend != 'undefined' && reply.entertosend);
                            if (enterToSend) {
                                isOkToPostReply = true;
                            }
                        }
                        if (isOkToPostReply) {
                            scope.showError = false;
                            //Prenvent flood
                            //if counting
                            if (floodTime != null) {
                                scope.floodError = true;
                                return;
                            }
                            var to = topic.message.to;

                            scope.isPosting = true;
                            var message = scope.reply.body.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, "<br/>");

                            // make promise
                            var defered = $q.defer();

                            // success create new topic
                            defered.promise.then(function (resp) {
                                // success goes here
                                //hide indicator
                                scope.isPosting = false;
                                // add reply to box
                                addReply(resp);
                            },
                            function (err) {
                                // error goes here
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while creating topic');
                            });

                            // save promise to scope
                            scope.defer = defered;
                            // submit form through iframe to post file
                            if (angular.isDefined(scope.iframe.submit)) {
                                scope.iframe.submit();
                            } else {
                                topicSrv.addReply({ tid: topic.topicId, from: username, to: to }, { message: message, filesList: scope.filesList })
                                  .then(function (dt) {
                                      // success goes here
                                      scope.isPosting = false;
                                      defered.resolve(dt);
                                  }, function (err) {
                                      //error goes here
                                      scope.isPosting = false;
                                      defered.reject(err);
                                      // Show error message
                                      myAlert.pop('e', 'Error!', 'There was an error occurred while adding reply');
                                  });
                            }
                        }
                    } else {
                        //show error
                        scope.showError = true;
                    }
                };

                //load older replies
                scope.loadOlderMessages = function (tp) {
                    scope.topicReplies.loadingOlderReplies = true;
                    topicSrv.getTopicReplies(username, tp.topicId, 'Old')
                        .then(function () {
                            scope.topicReplies.loadingOlderReplies = false;
                            Array.prototype.push.apply(scope.topicReplies.olderReplies, topicSrv.replies.replies);
                            //scope.topicReplies.olderReplies = dt.replies;

                            scope.topicReplies.showOlderReplies = true;
                        },
                        function (error) {
                            // TODO: error 
                            // Show error message
                            myAlert.pop('e', 'Error!', 'There was an error occurred while loading older messages');
                        }
                    );
                };

                //prevent new line when press enter
                scope.preventNewLine = function (reply, $event) {
                    if (reply.entertosend) {
                        $event.preventDefault();
                    }
                };

                //expand function
                scope.expandTopicBox = function ($event, tp) {
                    if (angular.element($event.target).hasClass('message-avatar') ||
                        angular.element($event.target).hasClass('message-username') ||
                        angular.element($event.target).hasClass('btnsavefile') ||
                        angular.element($event.target).hasClass('topic-box-loading-replies')) {
                        $event.preventDefault();
                        return;
                    }
                    if (angular.element(element).find(".topic-replies").hasClass("slidein")) {

                        // clear reploy box template url
                        //scope.topicReply.templateUrl = '';

                        // close reply box
                        topicBoxCollapsed(false);
                    } else {
                        //load Replies
                        // clear reply array list
                        scope.topicReplies.recentReplies = [];

                        // disable has old replies state
                        scope.topicReplies.hasOlderReplies = false;

                        // show loading replies status
                        scope.topicReplies.loadingReplies = true;

                        // do request get topic replies
                        topicSrv.getTopicReplies(username, tp.topicId, 'Recent')
                            .then(function () {
                                // set reply box template url
                                scope.topicReply.templateUrl = 'scripts/flowTasksApp/views/topic/box.reply.html';

                                Array.prototype.push.apply(scope.topicReplies.recentReplies, topicSrv.replies.replies);
                                //scope.topicReplies.recentReplies = dt.replies;

                                scope.topicReplies.loadingReplies = false;
                                scope.topicReplies.hasOlderReplies = topicSrv.replies.hasOldReplies;

                                if (scope.topic.status !== 'Read') {
                                    topicSrv.removeCache();
                                }
                                scope.topic.status = 'Read';

                            },
                            function (error) {
                                // TODO: error;
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while loading recent replies');
                            }
                        );
                    }
                };

                // on reply box load
                scope.onTopicReplyLoad = function () {
                    scope.isClear = true;

                    topicBoxCollapsed(true);
                };

                //view user info
                scope.viewUserInfo = function (userName) {
                    var params = { userName: userName, currentUserList: workContextSrv.getCurrentUser().following };
                    $rootScope.$broadcast("openPopupViewUserInfo", params);
                };

                scope.$on("$destroy", function () {
                    $document.unbind("click");
                });
            }
        };
    }])

    // Create topic directive
    .directive("createTopicBox", ['helperSrv', 'globalConfig', 'WorkContextSrv', 'TopicSrv', '$timeout', 'ApplicationFactory', '$q', 'myAlert', 'TopicFactory',
    function (helperSrv, globalConfig, workContextSrv, topicSrv, $timeout, applicationFactory, $q, myAlert, topicFactory) {
        return {
            replace: true,
            restrict: "E",
            scope: {
                addNewTopicToList: "&addNewTopicToList",
                newTopicModel: "=newTopicModel"
            },
            templateUrl: 'scripts/flowTasksApp/views/topic/createTopicBox.html',
            link: function (scope, element) {
                var username = applicationFactory.userData.userName;                                // Get current username
                var to = '';                                                                        // Topic Direct to

                // fn reset values and states
                var clear = function () {
                    // reset values
                    scope.createNewTopic.postUrl = topicFactory.createTopicUrl();
                    //hide indicator
                    scope.createNewTopic.onSubmitting = false;
                    scope.createNewTopic.defer = null;
                };

                // process the form after submit
                var processTheForm = function (resp) {
                    //turn off submitting state
                    scope.createNewTopic.onSubmitting = false;
                    scope.createNewTopic.showError = false
                    if (resp.result) {
                        // hide modal
                        angular.element('#createTopicBox').modal('hide');

                        // clear file array list
                        scope.createNewTopic.filesList = [];

                        // remove direct to status
                        scope.createNewTopic.isDirectTo = false;
                        angular.element("#directtoselector").removeClass("opened").hide();

                        // set form pristine
                        scope.createtopicform.$setPristine();

                        // reset new topic model
                        scope.createNewTopic.newtopic = { attachments: [], message: '', directTo: [] };

                        // fix IE
                        scope.createNewTopic.newtopic.directTo = [];
                        // fix IE
                        angular.element("#directtoblock").slideUp();


                        // remove file from queue
                        angular.element(element).find(".files").empty();
                    } else {
                        // show error
                        scope.createNewTopic.hasErrorWhilePosting = true;

                        // bind error message
                        scope.createNewTopic.errorMessage = resp.errorMessage;
                    }
                }

                // scope object
                scope.createNewTopic = {
                    newtopic: { attachments: [], message: '', directTo: [] },                       // New topic view model
                    onSubmitting: false,                                                            // On submiting state
                    showError: false,                                                               // The value indicating whether show error
                    hasErrorWhilePosting: false,                                                    // The value indicating whether has error while submiting new topic
                    errorMessage: '',                                                               // Error message
                    isDirectTo: false,                                                              // The value indicating whether direct to checked
                    filesList: [],                                                                  // Attachments array
                    defer: null,                                                                    // Promise
                    postUrl: topicFactory.createTopicUrl(),                                         // Request url
                    iframe: {},                                                                     // Contains functions was created from Iframe
                    userFollowing: applicationFactory.userData.follower,                            // User followings array
                    isClear: false,                                                                 // The value indicating whether file upload control reset
                    // Show create topic modal
                    createTopic: function () {
                        // Open modal
                        jQuery('#createTopicBox').modal('show');
                        this.isClear = false;
                    },
                    // Submit new topic
                    submitTopic: function (newtopic) {
                        // disable on submitting state
                        scope.createNewTopic.onSubmitting = false;

                        // hide error message
                        scope.createNewTopic.showError = true;

                        // check form is valid
                        if (scope.createtopicform.$valid && ((scope.isDirectTo && scope.newtopic.directTo.length) || (!scope.isDirectTo))) {

                            //turn on submitting state
                            scope.createNewTopic.onSubmitting = true;

                            // get selected user in direct to
                            var message = newtopic.message.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, "<br/>");

                            // make promise
                            var defered = $q.defer();

                            // success create new topic
                            defered.promise.then(function (resp) {
                                // success goes here

                                // bind new topic data to model
                                scope.newTopicModel = resp;

                                // append new topic to list
                                scope.addNewTopicToList({ newTopicModel: resp });

                                // process form
                                processTheForm(resp);

                                clear();
                            }, function (err) {
                                // error goes here
                                clear();
                                // Show error message
                                myAlert.pop('e', 'Error!', 'There was an error occurred while creating topic');
                            });

                            // save promise to scope
                            scope.createNewTopic.defer = defered;
                            // submit form through iframe to post file
                            if (angular.isDefined(scope.createNewTopic.iframe.submit)) {
                                // submit the iframe
                                scope.createNewTopic.iframe.submit();
                            } else {
                                // do add topic request
                                topicSrv.addTopic({ from: username, to: to }, { message: message, filesList: scope.createNewTopic.filesList })
                                    .then(function (dt) {
                                        // resolve the promise
                                        defered.resolve(dt);
                                    }, function (err) {
                                        // error goes here
                                        defered.reject(err);
                                        // Show error message
                                        myAlert.pop('e', 'Error!', 'There was an error occurred while creating topic');
                                    });
                            }

                        }
                    },
                }

                // uncheck direct to check box and hide direct to box when first load
                angular.element("#directtoselector").removeClass("opened").hide();

                // watch direct to change
                scope.$watch('createNewTopic.isDirectTo', function () {
                    if (scope.createNewTopic.isDirectTo) {

                    } else {
                        // clear direct to users
                        scope.createNewTopic.newtopic.directTo = [];
                    }
                });

                // watch new topic view model change then generate new url for submiting new topic
                scope.$watch('createNewTopic.newtopic', function () {
                    to = '';
                    // get selected user in direct to
                    for (var i = 0; i < scope.createNewTopic.newtopic.directTo.length; i++) {
                        to += scope.createNewTopic.newtopic.directTo[i].userName + ',';
                    }

                    //remove last comma
                    to = to.replace(/^,|,$/, '');

                    // set post url
                    scope.createNewTopic.postUrl = topicFactory.createTopicUrl(applicationFactory.userData.userName, to);

                }, true);

                // handle model close action
                jQuery('#createTopicBox').on('hidden.bs.modal', function () {
                    scope.$apply(function () {
                        // reset data
                        clear();

                        // reset data
                        scope.createNewTopic.isClear = true;

                        // clear file array list
                        scope.createNewTopic.filesList = [];

                        // disable on submitting state
                        scope.createNewTopic.onSubmitting = false;

                        // hide error message
                        scope.createNewTopic.showError = false;

                        // hide error message
                        scope.createNewTopic.hasErrorWhilePosting = false;

                        // remove error message
                        scope.createNewTopic.errorMessage = '';

                        // remove direct to status
                        scope.createNewTopic.isDirectTo = false;
                        angular.element("#directtoselector").removeClass("opened").hide();

                        // set form pristine
                        scope.createtopicform.$setPristine();

                        // reset new topic model
                        scope.createNewTopic.newtopic = { attachments: [], message: '', directTo: [] };

                        // clear files html
                        jQuery(element).find(".files").empty();

                        //  hide direct to block
                        jQuery("#directtoblock").slideUp();
                    });
                });
            }
        };
    }])

    // Download file directive
    .directive("downloadFile", [function () {
        return {
            scope: {
                file: '='
            },
            restrict: 'E',
            replace: true,
            controller: ['$scope', function ($scope) {
                //Save file
                //$scope.onDownloading = false;
                $scope.saveFile = function (file) {
                    var oid = typeof file.documentOid === 'undefined' ? file.oid : file.documentOid;
                    //$scope.onDownloading = true;
                    window.open('api/docs/' + oid);
                };
            }],
            template: '<b class="pull-left col-sm-6 btnsavefile" ng-click="saveFile(file)"><i class="fa fa-file"></i> {{file.fileName}}</b>'
        };
    }])

    // Multiple select directive
    .directive("selectMulti", [function () {
        return {
            restrict: "A",
            require: 'ngModel',
            link: function () {
                $(function () {

                });
            }
        };
    }])

    // collase topic when click outside
    .directive("collapseTopicClick", ['$document', function ($document) {
        return {
            restrict: "A",
            scope: {
                onSearching: "=onSearching",
                showSearch: "=showSearch"
            },
            link: function (scope) {
                //hide search result when click to outside
                $document.bind('click', function (e) {
                    if (angular.element(e.target).parents(".topic-search").length === 0) {
                        scope.onSearching = false;
                        scope.showSearch = false;
                    }
                });
            }
        }
    }])

    // Expand element
    .directive('expandElement', [function () {
        return {
            restrict: "A",
            link: function (scope, elem, attrs) {
                var $targetElement = angular.element('#' + attrs.expandElement);
                // hide target element
                $targetElement.hide();

                elem.on("click", function () {
                    if ($targetElement.hasClass("opened")) {
                        $targetElement.hide();
                        $targetElement.removeClass("opened");
                    } else {
                        $targetElement.show();
                        $targetElement.addClass("opened");
                    }
                });

                scope.$on("$destroy", function () {
                    elem.off("click");
                });
            }
        }
    }])
;