// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Support;

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
                        message: WorkflowJson.ToJson(
                            value: workflowRequest));

                FlowExecution flowExecution =
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
}