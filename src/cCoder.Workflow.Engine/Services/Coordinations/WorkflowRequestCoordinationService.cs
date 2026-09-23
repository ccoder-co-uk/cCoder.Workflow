// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Services.Coordinations;

internal sealed partial class WorkflowRequestCoordinationService(
    IFlowExecutionOrchestrationService flowExecutionOrchestrationService,
    IWorkflowLifecycleOrchestrationService workflowLifecycleOrchestrationService)
    : IWorkflowRequestCoordinationService
{
    public ValueTask ExecuteWorkflowRequestAsync(
        WorkflowRequest workflowRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowRequest]);

            FlowExecution flowExecution = CreateFlowExecution(
                workflowRequest: workflowRequest);

            await workflowLifecycleOrchestrationService
                .StartFlowExecutionAsync(
                    flowExecution: flowExecution);

            try
            {
                flowExecution = await flowExecutionOrchestrationService
                    .ExecuteFlowExecutionAsync(
                        flowExecution: flowExecution);

                await workflowLifecycleOrchestrationService
                    .SaveFlowExecutionAsync(
                        flowExecution: flowExecution);
            }
            catch (Exception exception)
            {
                await workflowLifecycleOrchestrationService
                    .RecordFlowExecutionFailureAsync(
                        flowExecution: flowExecution,
                        exception: exception);

                throw;
            }
            finally
            {
                await workflowLifecycleOrchestrationService
                    .FinishFlowExecutionAsync(
                        flowExecution: flowExecution);
            }
        });

    private static FlowExecution CreateFlowExecution(
        WorkflowRequest workflowRequest) =>
        new()
        {
            Request = workflowRequest
        };
}