// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Extensions;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal sealed partial class WorkflowRequestOrchestrationService(
    IFlowCommunicationProcessingService
        flowCommunicationProcessingService,
    IFlowInstanceProcessingService
        flowInstanceProcessingService,
    IFlowResultProcessingService
        flowResultProcessingService)
    : IWorkflowRequestOrchestrationService
{
    public ValueTask ExecuteWorkflowRequestAsync(
        WorkflowRequest workflowRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowRequest]);

            await flowCommunicationProcessingService
                .ConnectWorkflowRequestAsync(
                    workflowRequest: workflowRequest);

            FlowExecution flowExecution = null;

            try
            {
                await flowCommunicationProcessingService
                    .LogWorkflowRequestAsync(
                        workflowRequest: workflowRequest,
                        level: WorkflowLogLevel.Info,
                        message:
                            "Request received by workflow, "
                            + "processing ...");

                await flowCommunicationProcessingService
                    .LogWorkflowRequestAsync(
                        workflowRequest: workflowRequest,
                        level: WorkflowLogLevel.Debug,
                        message: ObjectExtensions.ToJson(
                            value: workflowRequest));

                flowExecution =
                    CreateFlowExecution(
                        workflowRequest: workflowRequest);

                flowExecution =
                    await flowInstanceProcessingService
                        .ExecuteFlowExecutionAsync(
                            flowExecution: flowExecution);

                await flowResultProcessingService
                    .SaveFlowInstanceDataAsync(
                        flowInstanceData: flowExecution.Result,
                        apiRoot: workflowRequest.Api,
                        authToken: workflowRequest.AuthToken);
            }
            catch (Exception exception)
            {
                try
                {
                    await PersistFailureAsync(
                        flowExecution: flowExecution,
                        workflowRequest: workflowRequest,
                        exception: exception);
                }
                catch (Exception failurePersistenceException)
                {
                    await flowCommunicationProcessingService
                        .LogWorkflowRequestAsync(
                            workflowRequest: workflowRequest,
                            level: WorkflowLogLevel.Error,
                            message:
                                "Workflow failure state could not be persisted."
                                + Environment.NewLine
                                + failurePersistenceException.Message);
                }

                await flowCommunicationProcessingService
                    .LogWorkflowRequestAsync(
                        workflowRequest: workflowRequest,
                        level: WorkflowLogLevel.Fatal,
                        message:
                            "Failed to process request, abandoning "
                            + $"execution{Environment.NewLine}"
                            + $"{exception.Message}"
                            + $"{Environment.NewLine}"
                            + exception.StackTrace);

                throw;
            }
            finally
            {
                await flowCommunicationProcessingService
                    .LogWorkflowRequestAsync(
                        workflowRequest: workflowRequest,
                        level: WorkflowLogLevel.Info,
                        message: "Done!");
            }
        });

    private FlowExecution CreateFlowExecution(
        WorkflowRequest workflowRequest) =>
        new()
        {
            Request = workflowRequest,
            Log = (level, message) =>
                flowCommunicationProcessingService
                    .LogWorkflowRequestAsync(
                        workflowRequest: workflowRequest,
                        level: level,
                        message: message)
                    .AsTask()
        };

    private async ValueTask PersistFailureAsync(
        FlowExecution flowExecution,
        WorkflowRequest workflowRequest,
        Exception exception)
    {
        if (flowExecution?.Result is null)
        {
            return;
        }

        WorkflowContext context =
            DeserializeContext(
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
                        $"{inner.Message}{Environment.NewLine}{inner.StackTrace}"));

            inner = inner.InnerException;
        }

        flowExecution.Result.State = "Failed";
        flowExecution.Result.End = DateTimeOffset.UtcNow;

        flowExecution.Result.ContextString =
            JsonConvert.SerializeObject(
                value: context,
                settings: ObjectExtensions.GetJsonSettings());

        await flowResultProcessingService.SaveFlowInstanceDataAsync(
            flowInstanceData: flowExecution.Result,
            apiRoot: workflowRequest.Api,
            authToken: workflowRequest.AuthToken);
    }

    private static WorkflowContext DeserializeContext(string contextString)
    {
        if (string.IsNullOrWhiteSpace(value: contextString))
        {
            return new WorkflowContext { ExecutionLog = [] };
        }

        try
        {
            return JsonConvert.DeserializeObject<WorkflowContext>(
                       value: contextString,
                       settings: ObjectExtensions.GetJsonSettings())
                   ?? new WorkflowContext { ExecutionLog = [] };
        }
        catch
        {
            return new WorkflowContext { ExecutionLog = [] };
        }
    }
}