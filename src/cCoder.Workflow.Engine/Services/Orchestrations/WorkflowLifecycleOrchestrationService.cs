// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

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

            Exception persistenceFailure = null;

            try
            {
                await PersistFailureAsync(
                    flowExecution: flowExecution,
                    exception: exception);
            }
            catch (Exception failurePersistenceException)
            {
                persistenceFailure = failurePersistenceException;
            }

            try
            {
                if (persistenceFailure is not null)
                {
                    await flowCommunicationService.LogWorkflowRequestAsync(
                        workflowRequest: flowExecution.Request,
                        level: WorkflowLogLevel.Error,
                        message: "Workflow failure state could not be persisted."
                            + Environment.NewLine + persistenceFailure.Message);
                }

                await flowCommunicationService.LogWorkflowRequestAsync(
                    workflowRequest: flowExecution.Request,
                    level: WorkflowLogLevel.Fatal,
                    message: "Failed to process request, abandoning execution"
                        + Environment.NewLine + exception);
            }
            catch (Exception loggingFailure)
            {
                throw new AggregateException(innerExceptions:
                    new[] { exception, persistenceFailure, loggingFailure }
                        .Where(predicate: failure => failure is not null));
            }

            if (persistenceFailure is not null)
            {
                throw new AggregateException(innerExceptions: [exception, persistenceFailure]);
            }
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
            throw new InvalidOperationException(message: "Workflow failure cannot be persisted without its instance.");
        }

        WorkflowContext context = DeserializeContext(
            contextString: flowExecution.Result.ContextString);

        if (context is null)
        {
            flowExecution.Result.State = "Failed";
            flowExecution.Result.End = DateTimeOffset.UtcNow;

            await flowResultService.SaveFlowInstanceDataAsync(
                flowInstanceData: flowExecution.Result,
                apiRoot: flowExecution.Request.Api,
                authToken: flowExecution.Request.AuthToken);

            return;
        }

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
            return null;
        }

        try
        {
            return flowResultService.Deserialize<WorkflowContext>(
                value: contextString);
        }
        catch
        {
            return null;
        }
    }
}