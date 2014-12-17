using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using Flow.Connectors.DocsOnFolder.Configuration;
using Flow.Library;
using Flow.Docs.Contract;
using Ninject;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;

namespace Flow.Connectors.DocsOnFolder
{
    /// <summary>
    /// Class implementing the windows service for processing renewal import files.
    /// </summary>
    public partial class DocsOnFolderService : ServiceBase
    {
        #region Constants

        private const string FATAL_EXCEPTION_THROWN =
           "A critical error occurred, the service will be shut down.";

        private const string EXCEPTION_THROWN =
           "An error occurred, see the exception details:";

        private const string PROCESSING_OUTSANDING_FILES =
           "Processing {0} outstanding files in the import folder ...";

        private const string PROCESSING_FILE = "Processing file: '{0}'";

        private const string PROCESSING_FILE_SUCCESS =
           "File '{0}' processed successfully.";

        private const string FILE_ACCESS_ERROR =
           "The file '{0}' could not be read from the file system.";

        private const string PROCESSING_FILE_FAILURE =
           "An error occurred while processing file '{0}'.\n" +
           "The error message returned is: {1}";

        private const string EXTENSION_ERROR = ".error";
        private const string FOLDER_PROCESSED = @"\Processed\";
        private const string FOLDER_FAILED = @"\Failed\";
        private const string FILE_FILTER_ALL = "*.*";
        private const string DATE_FOLDER_FORMAT = "yyyyMMdd";

        #endregion Constants

        #region Variables

        // Logger object.
        private static readonly ILog _log =
           LogManager.GetLogger(typeof(DocsOnFolderService));

        private FileSystemWatcher _importFilesWatcher;
        private string _importLocation;
        private string _archiveLocation;
        private bool _configurationInitialized;
        private string _result;

        public IFlowDocsDocument _processDocs;

        #endregion Variables

        #region Constructor

        /// <summary>
        /// Constructor, initilizes the components.
        /// </summary>
        public DocsOnFolderService()
        {
            InitializeComponent();

            // initialize the file watcher
            _importFilesWatcher = new FileSystemWatcher();
            _importFilesWatcher.EnableRaisingEvents = false;
        }

        #endregion Constructor

        #region Event Handlers

        /// <summary>
        /// Overrides the OnStart event handler for the service. Initializes the file
        /// system watcher and configures it for the configured location and filters.
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

            if (_configurationInitialized)
            {
                // first process all outstanding files in the import folder (this is done
                // BEFORE the file watcher is initialized).
                ProcessOutstandingFiles();

                // initialize the file watcher and let it take over from here.
                InitializeAndStartFileWatcher();
            }
        }

        /// <summary>
        /// Overrides the OnStop event of the service.
        /// </summary>
        protected override void OnStop()
        {
            // stop the file watcher first
            StopFileWatcher();

            // log the start of the service
            _log.Info("---> The " + ServiceName + " service has STOPPED <---");
        }

        /// <summary>
        /// Event handler that processes the file created event. 
        /// </summary>
        /// <param name="source">Source of the event - not used in this method.</param>
        /// <param name="e">Event arguments - hold the file name.</param>
        private void OnFileAdded(object source, FileSystemEventArgs e)
        {
            // log the even handler invocation
            _log.Info("File: " + e.FullPath + " " + e.ChangeType);

            DateTime fileReceived = DateTime.Now;
            while (true)
            {
                if (FileUploadCompleted(e.FullPath))
                {
                    // process the file that was just added
                    ProcessFile(new FileInfo(e.FullPath));
                    break;
                }

                // calculate the elapsed time and stop if the maximum retry
                // period has been reached       
                TimeSpan timeElapsed = DateTime.Now - fileReceived;
                if (timeElapsed.TotalSeconds > ConfigHelper.MaximumRetrySeconds)
                {
                    _log.Error(string.Format(FILE_ACCESS_ERROR, e.FullPath));
                    break;
                }

                Thread.Sleep(ConfigHelper.RetryDelayMilliseconds);
            }
        }

        #endregion Event Handlers

        #region Methods

        /// <summary>
        /// Initializes the locations as per configuration file. Creates new folders if
        /// not yet set up. 
        /// NOTE: this method stops the service if there is an exception thrown at this
        ///       stage.
        /// </summary>
        public void InitializeConfiguration()
        {
            _log.Info("Maximum retry = " + ConfigHelper.MaximumRetrySeconds + "s");
            _log.Info("Retry delay = " + ConfigHelper.RetryDelayMilliseconds + "ms");

            try
            {
                _configurationInitialized = false;
                _importLocation = ConfigHelper.ImportLocation;
                _archiveLocation = ConfigHelper.ArchiveLocation;

                _log.Info("Import location is:  " + _importLocation);
                _log.Info("Archive location is: " + _archiveLocation);

                // check for existence of these folders
                if (!Directory.Exists(_importLocation))
                {
                    Directory.CreateDirectory(_importLocation);
                    _log.Info("Import folder did not exist. Created new folder: " +
                              _importLocation);
                }
                if (!Directory.Exists(_archiveLocation))
                {
                    Directory.CreateDirectory(_archiveLocation);
                    _log.Info("Archive folder did not exist. Created new folder: " +
                              _archiveLocation);
                }

                // check the 'Processed' and 'Failed' subfolders and create if missing
                string location = _archiveLocation + FOLDER_PROCESSED;
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                    _log.Info("The 'Processed' folder did not exist. Created new folder: " +
                              location);
                }
                location = _archiveLocation + FOLDER_FAILED;
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                    _log.Info("The 'Failed' folder did not exist. Created new folder: " +
                              location);
                }

                // set the flag to indicate that the initialization was successfull
                _configurationInitialized = true;
            }
            catch (Exception ex)
            {
                _log.Fatal(FATAL_EXCEPTION_THROWN, ex);
                _configurationInitialized = false;

                // after logging the error stop the service
                Stop();
            }
        }

        public void InitializeInfrastructure()
        {
            try
            {
            }
            catch (Exception ex)
            {
                _log.Fatal(FATAL_EXCEPTION_THROWN, ex);
                _configurationInitialized = false;

                // after logging the error stop the service
                Stop();
            }
        }

        /// <summary>
        /// Initializes and starts the file system wathcer for the import location.
        /// </summary>
        private void InitializeAndStartFileWatcher()
        {
            _importFilesWatcher.Path = _importLocation;

            // watch all files
            _importFilesWatcher.Filter = FILE_FILTER_ALL;

            // Add an event handler.
            _importFilesWatcher.Created += new FileSystemEventHandler(OnFileAdded);

            // begin watching
            _importFilesWatcher.EnableRaisingEvents = true;

            // log the start of watching
            _log.Info("The file system watcher has been initialized and started.");
        }

        /// <summary>
        /// Stops the system file watcher.
        /// </summary>
        private void StopFileWatcher()
        {
            _importFilesWatcher.EnableRaisingEvents = false;

            // log the end of watching
            _log.Info("The file system watcher has been stopped.");
        }

        /// <summary>
        /// Processes all outstanding imports that may have accumulated while the service
        /// was stopped.
        /// </summary>
        private void ProcessOutstandingFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(_importLocation);
            FileInfo[] files = dir.GetFiles();

            _log.Info(string.Format(PROCESSING_OUTSANDING_FILES, files.Length));

            foreach (FileInfo file in files)
            {
                ProcessFile(file);
            }
        }

        /// <summary>
        /// Check if the file upload has been completed.
        /// </summary>
        /// <param name="fileName">The name of file to check.</param>
        /// <returns>true if the specified file has completed uploading and
        /// is ready to be processed.</returns>
        private bool FileUploadCompleted(string fileName)
        {
            // The check is the following: if the file can be opened for exclusive access
            // it means that the file is no longer locked by another process.
            try
            {
                using (FileStream fs = File.Open(
                   fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(FATAL_EXCEPTION_THROWN, ex);
                return false;
            }
        }

        /// <summary>
        /// Processes the file given by the FileInfo object. 
        /// </summary>
        /// <param name="file">File to process.</param>
        public void ProcessFile(FileInfo file)
        {
            _log.Info(string.Format(PROCESSING_FILE, file.Name));

            // process the new file: use a try-catch block to prevent the service from
            // shutting down
            try
            {
                // use the tool to process the string read from the file
                ImportFile import = new ImportFile(file.FullName, _processDocs);
                _result = import.UploadFile();

                // check the result of the processing
                if (string.IsNullOrEmpty(_result))
                {
                    // processing successful - move the file to the 'Processed' folder
                    _log.Info(string.Format(PROCESSING_FILE_SUCCESS, file.Name));
                    MoveFile(file, CreateDatedPath(_archiveLocation + FOLDER_PROCESSED));
                }
                else
                {
                    // processing failed - create an error message
                    string errorMsg = string.Format(
                       PROCESSING_FILE_FAILURE, file.Name, _result);

                    _log.Error(errorMsg);

                    // get the dated folder name
                    string datedPath = CreateDatedPath(_archiveLocation + FOLDER_FAILED);

                    // create error file paths before the associated file is moved
                    string errorFilePath = datedPath + file.Name;

                    // move the file to the 'Failed' folder and get its 'sequence number'
                    string sequence = MoveFile(file, datedPath);

                    // generate an additional file with error message
                    StreamWriter sw = File.CreateText(errorFilePath + sequence +
                                                      EXTENSION_ERROR);
                    sw.WriteLine(errorMsg);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(EXCEPTION_THROWN, ex);
            }
        }

        /// <summary>
        /// Moves the file specified by FileInfo to the location defined by path.
        /// </summary>
        /// <param name="file">File to move.</param>
        /// <param name="path">Target location.</param>
        /// <returns>String form of the sequence used or empty for the first file
        /// version.</returns>
        private string MoveFile(FileInfo file, string path)
        {
            int counter = 0;
            string targetPath = path + file.Name + "." + counter;

            try
            {
                // if the file does not yet exist at the target location, move it
                if (!File.Exists(targetPath))
                {
                    file.MoveTo(targetPath);
                }
                else
                {
                    do
                    {
                        counter++;
                        targetPath = path + file.Name + "." + counter;
                    } while (File.Exists(targetPath));

                    // move to the target path that was past checked
                    file.MoveTo(targetPath);
                }
            }
            catch (Exception ex)
            {
                _log.Error(EXCEPTION_THROWN, ex);
            }

            return "." + counter;
        }

        /// <summary>
        /// Returns a path to the dated folder to use to move files to. 
        /// </summary>
        /// <returns>Dated folder name.</returns>
        private string CreateDatedPath(string rootPath)
        {
            // first check if there is a dated folder and create one if not
            string datedPath = rootPath + DateTime.Now.Date.ToString(DATE_FOLDER_FORMAT) +
                               System.IO.Path.DirectorySeparatorChar;
            if (!Directory.Exists(datedPath))
            {
                Directory.CreateDirectory(datedPath);
            }

            return datedPath;
        }

        #endregion Methods
    }
}