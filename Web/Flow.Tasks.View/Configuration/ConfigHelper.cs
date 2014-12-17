using System;
using System.Configuration;
using System.Text;

namespace Flow.Tasks.View.Configuration
{
    /// <summary>
    /// Class providing convenience methods for the configuration settings.
    /// </summary>
    public class ConfigHelper
    {
        #region Constants

        private const string CFG_KEY_DOWNLOAD_LOCATION = "DownloadLocation";
        private const string CFG_KEY_WF_DOMAIN = "WorkflowDomain";
        private const string CFG_KEY_WF_CODE_SKETCH = "WorkflowCodeSketch";
        private const string CFG_KEY_WF_CODE_HOLIDAY = "WorkflowCodeHoliday";
        private const string CFG_KEY_WF_PROP_DOC = "WPDocumentOid";
        private const string CFG_KEY_WF_PROP_CODE = "WPWorkflowCode";

        #endregion Constants

        #region Properties

        /// <summary>
        /// Gets the download location parameter from the config file.
        /// </summary>
        public static string DownloadLocation
        {
            get { return AppSetting(CFG_KEY_DOWNLOAD_LOCATION); }
        }

        /// <summary>
        /// Get the workflow domain
        /// </summary>
        public static string WorkflowDomain
        {
            get { return AppSetting(CFG_KEY_WF_DOMAIN); }
        }

        /// <summary>
        /// Get the workflow code Sketch
        /// </summary>
        public static string WorkflowCodeSketch
        {
            get { return AppSetting(CFG_KEY_WF_CODE_SKETCH); }
        }

        /// <summary>
        /// Get the workflow code Holiday
        /// </summary>
        public static string WorkflowCodeHoliday
        {
            get { return AppSetting(CFG_KEY_WF_CODE_HOLIDAY); }
        }

        /// <summary>
        /// Get the workflow document oid property
        /// </summary>
        public static string WorkflowPropertyDocumentOid
        {
            get { return AppSetting(CFG_KEY_WF_PROP_DOC); }
        }

        /// <summary>
        /// Get the workflow document code property
        /// </summary>
        public static string WorkflowPropertyCode
        {
            get { return AppSetting(CFG_KEY_WF_PROP_CODE); }
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
                var sb = new StringBuilder();
                sb.Append("Cannot find configuration value for key '");
                sb.Append(key);
                sb.Append("'. ");
                throw new ApplicationException(sb.ToString());
            }
            return configValue;
        }

        #endregion Methods
    }
}