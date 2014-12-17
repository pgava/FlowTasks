using System.Configuration;
using System.IO;
using System;
using Flow.Connectors.DocsOnFolder.Configuration;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;
using Flow.Docs.Process;
using Flow.Docs.Contract.Message;
using Flow.Docs.Contract;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;

namespace Flow.Connectors.DocsOnFolder
{
    /// <summary>
    /// Class that encapsulates the import file name, name of the import and the data
    /// type derived from the file name.
    /// </summary>
    internal class ImportFile
    {
        #region Variables

        private string _fileName = string.Empty;
        private string _importName = string.Empty;
        private string _dataType = string.Empty;
        private IFlowDocsDocument _processDocs;
        private IFlowTasksService _serviceTasks;

        #endregion Variables

        #region Constructor

        /// <summary>
        /// Initializes the import name and data type from the import file name.
        /// </summary>
        /// <param name="fileName">Import file name.</param>
        public ImportFile(string fileName, IFlowDocsDocument processDocs, IFlowTasksService serviceTasks)
        {
            _serviceTasks = serviceTasks;
            _processDocs = processDocs;
            _fileName = fileName;
        }

        public ImportFile(string fileName, IFlowDocsDocument processDocs)
        {
            _processDocs = processDocs;
            _fileName = fileName;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the file name for this import.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets the name of this import.
        /// </summary>
        public string ImportName
        {
            get { return _importName; }
        }

        /// <summary>
        /// Gets the data type for this import.
        /// </summary>
        public string DataType
        {
            get { return _dataType; }
        }

        #endregion Properties

        #region Methods

        public string UploadFile()
        {
            try
            {
                var info = new DocumentInfo { DocumentName = Path.GetFileName(FileName), Path = Path.GetDirectoryName(FileName), Description = "desc-test", Owner = "owner", Version = 1 };

                var oid = _processDocs.UploadDocument(info, FileName, DocumentUploadMode.NewVersion);

                var startWorkflowRequest = new StartWorkflowRequest
                    {
                        Domain = ConfigHelper.WorkflowDomain,
                        WorkflowCode = ConfigHelper.WorkflowCode,
                        WfRuntimeValues = new WfProperty[] 
                        {
                            new WfProperty
                            {
                                Name = ConfigHelper.WorkflowProperty,
                                Type = PropertyType.FlowDoc.ToString(),
                                Value = oid.ToString()
                            }
                        }
                    };

                if (_serviceTasks != null)
                {
                    _serviceTasks.StartWorkflow(startWorkflowRequest);
                }
                else
                {
                    using (FlowTasksService proxy = new FlowTasksService())
                    {
                        var startWorkflowResponse = proxy.StartWorkflow(startWorkflowRequest);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion

    }
}