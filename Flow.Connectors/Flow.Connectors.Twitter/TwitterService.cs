using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using Flow.Tasks.Proxy;
using Flow.Library;
using System.Timers;
using Flow.Connectors.Twitter.Configuration;
using Flow.Tasks.Contract;

namespace Flow.Connectors.Twitter
{
    /// <summary>
    /// TwitterService
    /// </summary>
    public partial class TwitterService : ServiceBase
    {
        #region Constants

        private const string FATAL_EXCEPTION_THROWN =
           "A critical error occurred, the service will be shut down.";

        private const string EXCEPTION_THROWN =
           "An error occurred, see the exception details:";

        #endregion Constants

        #region Variables

        // Logger object.
        private static readonly ILog _log =
           LogManager.GetLogger(typeof(TwitterService));

        private static System.Timers.Timer _intervalTimer;

        #endregion Variables

        #region Constructor

        /// <summary>
        /// Constructor, initilizes the components.
        /// </summary>
        public TwitterService()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Event Handlers

        /// <summary>
        /// Overrides the OnStart event handler for the service. Initializes the timer.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected override void OnStart(string[] args)
        {
            // set current directory to assembly folder
            Environment.CurrentDirectory = Path.GetDirectoryName(
               Assembly.GetEntryAssembly().Location);

            // initialize the logger and log the start of the service. The logger must be
            // reinitilaized here because the service may have been restarted due to config
            // changes.
            XmlConfigurator.Configure();

            // log the start of the service
            _log.Info("---> The " + ServiceName + " service has STARTED <---");

            // read the configuration settings and initialize the file system
            InitializeConfiguration();

            InitializeInfrastructure();

            InitializeAndStartTimer();
        }

        /// <summary>
        /// Overrides the OnStop event of the service.
        /// </summary>
        protected override void OnStop()
        {
            // stop the timer
            StopTimer();

            _log.Info("---> The " + ServiceName + " service has STOPPED <---");
        }

        /// <summary>
        /// Event handler that processes the time event. 
        /// </summary>
        /// <param name="source">Source of the event - not used in this method.</param>
        /// <param name="e">Event arguments - hold the signal time.</param>
        private void OnCallTwitter(object source, ElapsedEventArgs e)
        {
            _log.Info("Time: " + e.SignalTime);

            var process = new ProcessTweet(new Tweet());
            process.DoProcess();
        }

        #endregion Event Handlers

        #region Methods

        /// <summary>
        /// NOTE: this method stops the service if there is an exception thrown at this
        ///       stage.
        /// </summary>
        public void InitializeConfiguration()
        {
            try
            {
            }
            catch (Exception ex)
            {
                _log.Fatal(FATAL_EXCEPTION_THROWN, ex);

                // after logging the error stop the service
                Stop();
            }
        }

        /// <summary>
        /// Initialize Infrastructure
        /// </summary>
        public void InitializeInfrastructure()
        {
            try
            {
            }
            catch (Exception ex)
            {
                _log.Fatal(FATAL_EXCEPTION_THROWN, ex);

                // after logging the error stop the service
                Stop();
            }
        }

        /// <summary>
        /// Initializes and starts the timer.
        /// </summary>
        /// 
        private void InitializeAndStartTimer()
        {
            // Create a timer with a ten second interval. 
            _intervalTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer. 
            _intervalTimer.Elapsed += new ElapsedEventHandler(OnCallTwitter);

            // Set the Interval to delay seconds. 
            _intervalTimer.Interval = ConfigHelper.Delay * 1000;
            _intervalTimer.Enabled = true; 

            // log the start of timer
            _log.Info("The timer has been initialized and started.");
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void StopTimer()
        {
            _intervalTimer.Stop();

            // log the end of timer
            _log.Info("Timer has been stopped.");
        }

        #endregion Methods
    }
}