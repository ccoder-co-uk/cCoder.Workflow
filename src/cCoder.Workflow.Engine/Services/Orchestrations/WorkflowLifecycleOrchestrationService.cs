// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal sealed partial class WorkflowLifecycleOrchestrationService(
    IFlowCommunicationService flowCommunicationService,
    IFlowResultService flowResultService)
    : IWorkflowLifecycleOrchestrationService
{
    public ValueTask StartFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);
            WorkflowRequest request = flowExecution.Request;

            flowExecution.Log = (level, message) =>
                flowCommunicationService.LogWorkflowRequestAsync(
                    workflowRequest: request,
                    level: level,
                    message: message)
                .AsTask();

            await flowCommunicationService.ConnectWorkflowRequestAsync(
                workflowRequest: request);

            await flowCommunicationService.LogWorkflowRequestAsync(
                workflowRequest: request,
                level: WorkflowLogLevel.Info,
                message: "Request received by workflow, processing ...");

            await flowCommunicationService.LogWorkflowRequestAsync(
                workflowRequest: request,
                level: WorkflowLogLevel.Debug,
                message: flowResultService.Serialize(value: request));
        });

    public ValueTask SaveFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            await flowResultService.SaveFlowInstanceDataAsync(
                flowInstanceData: flowExecution.Result,
                apiRoot: flowExecution.Request.Api,
                authToken: flowExecution.Request.AuthToken);
        });

    public ValueTask RecordFlowExecutionFailureAsync(
        FlowExecution flowExecution,
        Exception exception) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution, exception]);

            try
            {
                await PersistFailureAsync(
                    flowExecution: flowExecution,
                    exception: exception);
            }
            catch (Exception failurePersistenceException)
            {
                await flowCommunicationService.LogWorkflowRequestAsync(
                    workflowRequest: flowExecution.Request,
                    level: WorkflowLogLevel.Error,
                    message:
                        "Workflow failure state could not be persisted."
                        + Environment.NewLine
                        + failurePersistenceException.Message);
            }

            await flowCommunicationService.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: WorkflowLogLevel.Fatal,
                message:
                    "Failed to process request, abandoning execution"
                    + Environment.NewLine
                    + exception.Message
                    + Environment.NewLine
                    + exception.StackTrace);
        });

    public ValueTask FinishFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            await flowCommunicationService.LogWorkflowRequestAsync(
                workflowRequest: flowExecution.Request,
                level: WorkflowLogLevel.Info,
                message: "Done!");
        });

    private async ValueTask PersistFailureAsync(
        FlowExecution flowExecution,
        Exception exception)
    {
        if (flowExecution.Result is null)
        {
            return;
        }

        WorkflowContext context = DeserializeContext(
            contextString: flowExecution.Result.ContextString);

        context.ExecutionState = "Failed";
        context.ExecutionLog = context.ExecutionLog?.ToList() ?? [];

        context.ExecutionLog.Add(
            item: new WorkflowLogEntry(
                level: WorkflowLogLevel.Error,
                message:
                    $"Workflow execution failed: {exception.Message}"
                    + $"{Environment.NewLine}{exception.StackTrace}"));

        Exception inner = exception.InnerException;

        while (inner is not null)
        {
            context.ExecutionLog.Add(
                item: new WorkflowLogEntry(
                    level: WorkflowLogLevel.Error,
                    message:
                        $"{inner.Message}{Environment.NewLine}"
                        + inner.StackTrace));

            inner = inner.InnerException;
        }

        flowExecution.Result.State = "Failed";
        flowExecution.Result.End = DateTimeOffset.UtcNow;

        flowExecution.Result.ContextString =
            flowResultService.Serialize(value: context);

        await flowResultService.SaveFlowInstanceDataAsync(
            flowInstanceData: flowExecution.Result,
            apiRoot: flowExecution.Request.Api,
            authToken: flowExecution.Request.AuthToken);
    }

    private WorkflowContext DeserializeContext(string contextString)
    {
        if (string.IsNullOrWhiteSpace(value: contextString))
        {
            return new WorkflowContext { ExecutionLog = [] };
        }

        try
        {
            return flowResultService.Deserialize<WorkflowContext>(
                value: contextString)
                ?? new WorkflowContext { ExecutionLog = [] };
        }
        catch
        {
            return new WorkflowContext { ExecutionLog = [] };
        }
    }
}