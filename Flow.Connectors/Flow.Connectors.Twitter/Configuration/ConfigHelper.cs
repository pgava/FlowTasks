using System;
using System.Configuration;
using System.IO;
using System.Text;
using log4net;

namespace Flow.Connectors.Twitter.Configuration
{
    /// <summary>
    /// Class providing convenience methods for the configuration settings.
    /// </summary>
    internal class ConfigHelper
    {
        #region Constants

        private const string CFG_FILE = "Flow.Connectors.Twitter.exe.config";
        private const string CFG_KEY_CONSUMER_KEY = "ConsumerKey";
        private const string CFG_KEY_CONSUMER_SECRET = "ConsumerSecret";
        private const string CFG_KEY_ACCESS_TOKEN = "AccessToken";
        private const string CFG_KEY_ACCESS_TOKEN_SECRET = "AccessTokenSecret";
        private const string CFG_KEY_WF_DOMAIN = "WorkflowDomain";
        private const string CFG_KEY_WF_CODE = "WorkflowCode";
        private const string CFG_KEY_WF_PROP = "WorkflowProperty";
        private const string CFG_KEY_TWITTER_USER = "TwitterUser";
        private const string CFG_KEY_TWITTER_PATTERN = "TwitterPattern";
        private const string CFG_KEY_DELAY = "Delay";
        private const string CFG_KEY_PATH_TWITTER_DATA = "PathTwitterData";

        private const int DEFAULT_MAX_RETRY_SECONDS = 60;
        private const int DEFAULT_RETRY_DELAY_MS = 1000;

        #endregion Constants

        #region Variables

        private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigHelper));

        #endregion Variables

        #region Properties

        public static string ConsumerKey
        {
            get { return AppSetting(CFG_KEY_CONSUMER_KEY); }
        }

        public static string ConsumerSecret
        {
            get { return AppSetting(CFG_KEY_CONSUMER_SECRET); }
        }

        public static string AccessToken
        {
            get { return AppSetting(CFG_KEY_ACCESS_TOKEN); }
        }

        public static string AccessTokenSecret
        {
            get { return AppSetting(CFG_KEY_ACCESS_TOKEN_SECRET); }
        }

        /// <summary>
        /// Get the workflow domain
        /// </summary>
        public static string WorkflowDomain
        {
            get { return AppSetting(CFG_KEY_WF_DOMAIN); }
        }

        /// <summary>
        /// Get the workflow domain
        /// </summary>
        public static string WorkflowCode
        {
            get { return AppSetting(CFG_KEY_WF_CODE); }
        }

        /// <summary>
        /// Get the workflow domain
        /// </summary>
        public static string WorkflowProperty
        {
            get { return AppSetting(CFG_KEY_WF_PROP); }
        }

        /// <summary>
        /// Get the twitter user where to read the tweet
        /// </summary>
        public static string TwitterUser
        {
            get { return AppSetting(CFG_KEY_TWITTER_USER); }
        }

        /// <summary>
        /// Get the pattern in the twitter to start the wf
        /// </summary>
        public static string TwitterPattern
        {
            get { return AppSetting(CFG_KEY_TWITTER_PATTERN); }
        }

        /// <summary>
        /// Get the delay in seconds between twitter calls
        /// </summary>
        public static int Delay
        {
            get { return int.Parse(AppSetting(CFG_KEY_DELAY)); }
        }

        /// <summary>
        /// Get the path where to find the xml file with twitter data
        /// </summary>
        public static string PathTwitterData
        {
            get { return AppSetting(CFG_KEY_PATH_TWITTER_DATA); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get a configuration setting for the specified key. Throws an exception if the
        /// value can't be found. This is because either the config file can not be found
        /// or an entry for the given key does not exist.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>Value for the specified key.</returns>
        private static string AppSetting(string key)
        {
            string configValue = ConfigurationManager.AppSettings[key];
            if (configValue == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Cannot find configuration value for key '");
                sb.Append(key);
                sb.Append("'. ");
                sb.Append(CFG_FILE);
                if (!File.Exists(CFG_FILE))
                {
                    sb.Append(" file missing from ");
                    sb.Append(Directory.GetCurrentDirectory());
                }
                else
                {
                    sb.Append(" file does not contain this key.");
                }
                _log.Error(sb.ToString());
                throw new ApplicationException(sb.ToString());
            }
            return configValue;
        }

        #endregion Methods
    }
}