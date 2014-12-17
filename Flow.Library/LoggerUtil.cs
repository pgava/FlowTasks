using System;
using System.Xml;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace Flow.Library
{
    /// <summary>
    /// Logging utility methods for Log4Net
    /// </summary>
    public class LoggerUtil
    {
        #region Variables

        /// <summary>
        /// Is Logging Configured
        /// </summary>
        private static bool _isLoggingConfigured;

        /// <summary>
        /// Config File Name
        /// </summary>
        private const string CONFIG_FILE_NAME = "log4net.config";

        #endregion Variables

        #region Public Methods

        /// <summary>
        /// Configures the logging for this process
        /// </summary>
        public static void ConfigureLogging()
        {
            if (_isLoggingConfigured)
            {
                return;
            }

            var log4NetConfig = new XmlDocument();

            log4NetConfig.Load(GetConfig());

            XmlConfigurator.Configure(log4NetConfig.DocumentElement);
            _isLoggingConfigured = true;
        }

        /// <summary>
        /// Get Config
        /// </summary>
        /// <returns>File name</returns>
        public static string GetConfig()
        {
            string folderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""));
            if (folderName == null) throw new Exception("Couldn't get base root folder.");

            string fileName = Path.Combine(folderName, CONFIG_FILE_NAME);
            while (!File.Exists(fileName))
            {
                // File not found, so determine the name of the parent folder
                int index = folderName.LastIndexOf(Path.DirectorySeparatorChar);
                if (index <= 0)
                {
                    throw new Exception("No config file found in " + folderName + " or parent folders");
                }
                folderName = folderName.Substring(0, index);
                fileName = Path.Combine(folderName, CONFIG_FILE_NAME); 
            }

            return fileName;
        }

        #endregion Public Methods
    }
}
