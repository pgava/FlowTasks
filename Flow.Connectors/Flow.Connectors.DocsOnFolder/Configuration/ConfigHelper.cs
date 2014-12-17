using System;
using System.Configuration;
using System.IO;
using System.Text;
using log4net;

namespace Flow.Connectors.DocsOnFolder.Configuration
{
    /// <summary>
    /// Class providing convenience methods for the configuration settings.
    /// </summary>
    internal class ConfigHelper
    {
        #region Constants

        private const string CFG_FILE = "DocsOnFolder.exe.config";
        private const string CFG_KEY_IMPORT_LOCATION = "ImportLocation";
        private const string CFG_KEY_ARCHIVE_LOCATION = "ArchiveLocation";
        private const string CFG_KEY_EXTRACT_LOCATION = "ExtractLocation";
        private const string CFG_KEY_MAX_RETRY_SECONDS = "MaximumRetrySeconds";
        private const string CFG_KEY_RETRY_DELAY_MS = "RetryDelayMilliseconds";
        private const string CFG_KEY_WF_DOMAIN = "WorkflowDomain";
        private const string CFG_KEY_WF_CODE = "WorkflowCode";
        private const string CFG_KEY_WF_PROP = "WorkflowProperty";

        private const int DEFAULT_MAX_RETRY_SECONDS = 60;
        private const int DEFAULT_RETRY_DELAY_MS = 1000;

        #endregion Constants

        #region Variables

        private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigHelper));

        #endregion Variables

        #region Properties

        /// <summary>
        /// Gets the import location parameter from the config file.
        /// </summary>
        public static string ImportLocation
        {
            get { return AppSetting(CFG_KEY_IMPORT_LOCATION); }
        }

        /// <summary>
        /// Gets the archive location parameter from the config file.
        /// </summary>
        public static string ArchiveLocation
        {
            get { return AppSetting(CFG_KEY_ARCHIVE_LOCATION); }
        }

        /// <summary>
        /// Gets the extract location parameter from the config file.
        /// </summary>
        public static string ExtractLocation
        {
            get { return AppSetting(CFG_KEY_EXTRACT_LOCATION); }
        }

        /// <summary>
        /// Gets the maximum time [in seconds] that the file watcher should keep retrying
        /// to access the import file. If the value is not found in the config file or if
        /// it is invalid, the default value of 60 seconds will be used.
        /// </summary>
        public static int MaximumRetrySeconds
        {
            get
            {
                int result = DEFAULT_MAX_RETRY_SECONDS;
                string cfgValue = AppSetting(CFG_KEY_MAX_RETRY_SECONDS);
                if (!string.IsNullOrEmpty(cfgValue))
                {
                    // If the parse is unsuccessful, revert back to default.
                    if (!int.TryParse(cfgValue, out result))
                    {
                        result = DEFAULT_MAX_RETRY_SECONDS;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the retry delay [in milliseconds] that the process will wait before
        /// trying to access the file again. If the value is not found in the config
        /// file or if it is invalid, the default value of 1000ms will be used.
        /// </summary>
        public static int RetryDelayMilliseconds
        {
            get
            {
                int result = DEFAULT_RETRY_DELAY_MS;
                string cfgValue = AppSetting(CFG_KEY_RETRY_DELAY_MS);
                if (!string.IsNullOrEmpty(cfgValue))
                {
                    // If the parse is unsuccessful, revert back to default.
                    if (!int.TryParse(cfgValue, out result))
                    {
                        result = DEFAULT_RETRY_DELAY_MS;
                    }
                }
                return result;
            }
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