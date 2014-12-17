using System.Activities;
using System.ServiceModel.Activities;
using System.ServiceModel;
using System.Xml;

namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Send & Receice base activity
    /// </summary>
    /// <remarks>
    /// CorrelationId is used when you want to execute this activity in parallel with
    /// other activities. Unfortunatelly WF4 generate the bookmark name like:
    /// --> ServiceContractName + OperationName
    /// So you cannot use the same OperationName. This activity will postfix the 
    /// CorrelationId to the OperationName.
    /// 
    /// MessageNamespace used for correlation query
    ///   Ex. [DataContract(Namespace = "urn:WF4Sample:startworkflowrequest")]
    ///  
    /// GenerateCorrelationId if this is set to true then it expect that the activity
    /// will generate the ID used for the correlation. The ID will then be used in 
    /// the SendReplay. If this is false the ID is read from the request message and
    /// used in the Receive.
    /// The ID is supposed to be in a field called 'WorkflowId'
    /// [DataContract(Namespace = "urn:WF4Sample:startworkflowrequest")]
    /// public class StartWorkflowRequest
    /// {
    ///         [DataMember]
    /// public string WorkflowId { get; set; }
    /// 
    /// UseContentCorrelation set this to false only if you want to use Context
    /// Correlation. 
    /// Context Correlation can be turned on like this:
    /// <system.serviceModel>
    /// <protocolMapping>
    /// <add scheme="http" binding="wsHttpContextBinding"/>
    /// </protocolMapping>
    /// </system.serviceModel>
    /// </remarks>
    /// <typeparam name="TRequest">Request</typeparam>
    /// <typeparam name="TResponse">Resopnse</typeparam>
    public class SendReceiveBase<TRequest, TResponse> : NativeActivity
    {
        /// <summary>
        /// Receice activity
        /// </summary>
        private Receive _receive;

        /// <summary>
        /// Send activity
        /// </summary>
        private SendReply _sendReply;

        /// <summary>
        /// Contract name like for a WCF service
        /// </summary>
        public string ServiceContractName { get; set; }
        
        /// <summary>
        /// Can Create Instance
        /// </summary>
        /// <remarks>
        /// Will create a workflow instance if true
        /// </remarks>
        public bool CanCreateInstance { get; set; }

        /// <summary>
        /// Operation Name Format
        /// </summary>
        /// <remarks>
        /// [Operation Name] + [Correlation Id]
        /// </remarks>
        private const string OPERATION_NAME_FORMAT = "{0}{1}";

        /// <summary>
        /// Formatted Operation Name
        /// </summary>
        private string _operationName = "";

        /// <summary>
        /// Store the operation name specified
        /// </summary>
        /// <remarks>
        /// Without correlation id
        /// </remarks>
        private string _operationNameRoot = "";

        /// <summary>
        /// Operation Name
        /// </summary>
        public string OperationName
        {
            get
            {
                return _operationNameRoot;
            }
            set
            {
                _operationNameRoot = value;
                _operationName = string.Format(OPERATION_NAME_FORMAT, value, _correlationId);
                _correlationOn = CreateCorrelationQuery();
            }
        }

        /// <summary>
        /// Used to generate the correlation hash
        /// </summary>
        private MessageQuerySet _correlationOn;

        /// <summary>
        /// Store the message namesapce
        /// </summary>
        private string _messageNamespace = "";

        /// <summary>
        /// Message namesapce
        /// </summary>
        public string MessageNamespace
        {
            get
            {
                return _messageNamespace;
            }
            set
            {
                _messageNamespace = value;
                _correlationOn = CreateCorrelationQuery();
            }
        }

        /// <summary>
        /// Store the correlation id
        /// </summary>
        private int _correlationId = 1;

        /// <summary>
        /// Correlation Id
        /// </summary>
        public int CorrelationId
        {
            get
            {
                return _correlationId;
            }
            set
            {
                _correlationId = value;
                _operationName = string.Format(OPERATION_NAME_FORMAT, _operationNameRoot, value);
                _correlationOn = CreateCorrelationQuery();
            }
        }

        /// <summary>
        /// Can Induce Idle
        /// </summary>
        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        /// <summary>
        /// Request
        /// </summary>
        private readonly Variable<TRequest> _request = new Variable<TRequest> { Name = "request" };

        /// <summary>
        /// Response
        /// </summary>
        private readonly Variable<TResponse> _response = new Variable<TResponse> { Name = "response" };

        /// <summary>
        /// Associates activities together
        /// </summary>
        public InArgument<CorrelationHandle> CorrelationHandle;

        /// <summary>
        /// OnCompleting callback 
        /// </summary>
        public ActivityFunc<TRequest, TResponse> OnCompleting { get; set; }
        
        /// <summary>
        /// Generate Correlation Id
        /// </summary>
        /// <remarks>
        /// If this is set to true then it expect that the activity will generate the ID
        /// used for the correlation. The ID will then be used in the SendReplay. 
        /// If this is false the ID is read from the request message and
        /// used in the Receive.
        /// </remarks>
        public bool GenerateCorrelationId { get; set; }

        /// <summary>
        /// Use Content Correlation
        /// </summary>
        /// <remarks>
        /// Set this to false only if you want to use Context Correlation.
        /// </remarks>
        public bool UseContentCorrelation { get; set; }

        /// <summary>
        /// Create activity metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Create receive
            _receive = CreateReceive();
            
            // Create sendreplay
            _sendReply = CreateSendReplay();

            // Set up the activity metadata
            SetupMetadata(metadata);

        }

        /// <summary>
        /// Create Receive
        /// </summary>
        /// <returns>Receive activity</returns>
        private Receive CreateReceive()
        {
            _receive = _receive ?? new Receive();
            _receive.CanCreateInstance = CanCreateInstance;
            _receive.ServiceContractName = ServiceContractName;
            _receive.OperationName = _operationName;
            var args = new ReceiveParametersContent();
            args.Parameters["request"] = new OutArgument<TRequest>(_request);
            _receive.Content = args;
            if (!GenerateCorrelationId && UseContentCorrelation)
            {
                _receive.CorrelatesWith = CorrelationHandle;
                _receive.CorrelatesOn = _correlationOn;
            }


            return _receive;
        }

        /// <summary>
        /// Create SendReplay 
        /// </summary>
        /// <returns>SendReply activity</returns>
        private SendReply CreateSendReplay()
        {
            _sendReply = _sendReply ?? new SendReply();
            var parameters = new SendParametersContent();
            parameters.Parameters["response"] = new InArgument<TResponse>(_response);
            _sendReply.Content = parameters;
            _sendReply.Request = _receive;
            if (GenerateCorrelationId && UseContentCorrelation)
            {
                _sendReply.CorrelationInitializers.Clear();
                _sendReply.CorrelationInitializers.Add(
                    new QueryCorrelationInitializer
                    {
                        CorrelationHandle = CorrelationHandle,
                        MessageQuerySet = _correlationOn,
                    }
                );
            }

            return _sendReply;
        }

        /// <summary>
        /// Add activities, variables and argument to metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        private void SetupMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddImplementationVariable(_request);
            metadata.AddImplementationVariable(_response);
            metadata.AddDelegate(OnCompleting);
            var runtimeArgument = new RuntimeArgument("WfCorrelation", typeof(CorrelationHandle), ArgumentDirection.In);
            metadata.Bind(CorrelationHandle, runtimeArgument);
            metadata.AddArgument(runtimeArgument);
            metadata.AddImplementationChild(_receive);
            metadata.AddImplementationChild(_sendReply);

        }

        /// <summary>
        /// Activity entry point
        /// </summary>
        /// <param name="context">Activity context</param>
        protected override void Execute(NativeActivityContext context)
        {
            context.ScheduleActivity(_receive, ReceiveCompleted);
        }

        /// <summary>
        /// Child Completion Callback
        /// </summary>
        /// <remarks>
        /// Run after the client OnCompleting callback.
        /// </remarks>
        /// <param name="context">Context</param>
        /// <param name="completedInstance">ActivityInstance</param>
        /// <param name="result">Response</param>
        private void ChildCompletionCallback(NativeActivityContext context, ActivityInstance completedInstance, TResponse result)
        {
            //Set the output for the parent activity at the completion of the child activity 
            _response.Set(context, result);

            context.ScheduleActivity(_sendReply);
        }

        /// <summary>
        /// Receive Completed callback
        /// </summary>
        /// <remarks>
        /// Run soon after a message is received.
        /// </remarks>
        /// <param name="context">Context</param>
        /// <param name="completedInstance">ActivityInstance</param>
        private void ReceiveCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (OnCompleting != null)
            {
                context.ScheduleFunc(OnCompleting, _request.Get(context), ChildCompletionCallback);
            }
            else
            {
                context.ScheduleActivity(_sendReply);
            }

        }

        /// <summary>
        /// Generate the correlation hash
        /// </summary>
        /// <returns>MessageQuerySet</returns>
        private MessageQuerySet CreateCorrelationQuery() 
        {
            var xpath = new XPathMessageQuery
                            {
                Namespaces = new XmlNamespaceManager(new NameTable()),
                Expression = "//sample:WorkflowId"
            };
            xpath.Namespaces.AddNamespace("sample", MessageNamespace); 
                        
            var messageQuerySet = new MessageQuerySet
                                      { 
                Name = "RequestCorrelation" 
            }; 
            messageQuerySet.Add("WorkflowId", xpath);

            return messageQuerySet; 
        }

    }
}
