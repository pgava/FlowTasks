Imports System.Activities
Imports Flow.Tasks.Workflow.Activities
Imports System.Reflection
Imports log4net
Imports System.Diagnostics.CodeAnalysis

Public Class MyApproveTaskVb
    Inherits Activity

    Private _onInit As DelegateInArgument(Of TaskStateData) = New DelegateInArgument(Of TaskStateData)()
    Private _onComplete As DelegateInArgument(Of TaskStateData) = New DelegateInArgument(Of TaskStateData)()

    Private _correlationId As Integer
    Public Property CorrelationId() As Integer
        Get
            Return _correlationId
        End Get
        Set(value As Integer)
            _correlationId = value
        End Set
    End Property

    Private _taskCode As String
    Public Property TaskCode() As String
        Get
            Return _taskCode
        End Get
        Set(value As String)
            _taskCode = value
        End Set
    End Property

    <SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")>
    Public Sub New()
        Implementation = Function()
                             Return New ApproveTask With _
                                    { _
                                        .AssignedToUsers = "{r.Dev}",
                                        .CorrelationId = CorrelationId,
                                        .DefaultResult = "Activity Expired",
                                        .Description = "This is desc for my approve task",
                                        .DisplayName = "Approve my  task",
                                        .TaskCode = TaskCode,
                                        .Title = "This is the title for my approve",
                                        .UiCode = "ApproveTask",
                                        .ExpiresIn = "10d",
                                        .OnInit = New ActivityFunc(Of TaskStateData, TaskStateData) With _
                                                  { _
                                                      .Argument = _onInit,
                                                      .Handler = New CreateOnClientInit With _
                                                        { _
                                                            .DisplayName = "CreateOnClientInit",
                                                            .Request = _onInit
                                                        } _
                                                  }, _
                                        .OnComplete = New ActivityFunc(Of TaskStateData, TaskStateData) With _
                                                  { _
                                                      .Argument = _onComplete,
                                                      .Handler = New CreateOnClientComplete With _
                                                        { _
                                                            .DisplayName = "CreateOnClientComplete",
                                                            .Request = _onComplete
                                                        } _
                                                  } _
                                    }
                         End Function
    End Sub
End Class

Class CreateOnClientInit
    Inherits CodeActivity(Of TaskStateData)

    Private Shared log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)
    Private _request As InArgument(Of TaskStateData)

    Public Property Request() As InArgument(Of TaskStateData)
        Get
            Return _request
        End Get
        Set(value As InArgument(Of TaskStateData))
            _request = value
        End Set
    End Property

    Protected Overrides Function Execute(context As CodeActivityContext) As TaskStateData
        log.Debug("CreateOnClientInit -> Start")

        Dim taskStatus = Request.Get(context)

        taskStatus.AddParameter("TaskProp1", "TaskVal1")

        log.Debug("CreateOnClientInit -> End")

        Return taskStatus

    End Function
End Class

Class CreateOnClientComplete
    Inherits NativeActivity(Of TaskStateData)

    Private Shared log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)
    Private _request As InArgument(Of TaskStateData)

    Public Property Request() As InArgument(Of TaskStateData)
        Get
            Return _request
        End Get
        Set(value As InArgument(Of TaskStateData))
            _request = value
        End Set
    End Property

    Protected Overrides Sub Execute(context As NativeActivityContext)
        log.Debug("CreateOnClientComplete -> Start")

        Dim taskStatus = Request.Get(context)
        WorkflowAction.SetWorkflowResult(context, "OK")
        Result.Set(context, taskStatus)

        log.Debug("CreateOnClientComplete -> End")
    End Sub
End Class
