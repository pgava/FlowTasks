// Karma configuration
// Generated on Fri Nov 28 2014 09:51:34 GMT+1100 (AUS Eastern Daylight Time)

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],


    // list of files / patterns to load in the browser
    files: [
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular.min.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/angular-mocks.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/js/*.js',
      '../../../Web/Flow.Tasks.Web/Scripts/jquery-2.1.0.js',
      '../../../Web/Flow.Tasks.Web/Scripts/global.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/*.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/factories/*.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/services/*.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/controllers/*.js',
      '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/directives/*.js',
      '../../../Web/Flow.Tasks.Web/Content/themes/smart/js/datetime/*.js',
      '../../../Web/Flow.Tasks.Web/scripts/flowTasksApp/views/**/*.html',
      'specs/topicSrv.test.js',
      'specs/holidayCtrl.test.js',
      'specs/dashboardCtrl.test.js',
      'specs/taskCtrl.test.js',
      'specs/topicCtrl.test.js',
      'specs/userCtrl.test.js',
      'specs/userDirv.test.js'
  ],

    

    // list of files to exclude
    exclude: [
    '../../../Web/Flow.Tasks.Web/Scripts/flowtasksApp/router.js'
    ],

    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        '../../../Web/Flow.Tasks.Web/scripts/flowTasksApp/views/**/*.html': ['ng-html2js']
    },

    ngHtml2JsPreprocessor: {
        cacheIdFromPath: function(filepath) {
            return filepath.match(/scripts\/flowTasksApp\/views\/.*\/.*\.html/);
        },
        moduleName: 'flowTasksDirectives'
    },

    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome'],

    plugins: [
          'karma-chrome-launcher',
          'karma-jasmine',
          'karma-ng-html2js-preprocessor'
    ],

    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false
  });
};
