// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

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

            bool attemptConflict = false;

            try
            {
                flowExecution = await flowExecutionOrchestrationService
                    .ExecuteFlowExecutionAsync(
                        flowExecution: flowExecution);

                await workflowLifecycleOrchestrationService
                    .SaveFlowExecutionAsync(
                        flowExecution: flowExecution);
            }
            catch (Exception exception) when (IsAttemptConflict(exception: exception))
            {
                attemptConflict = true;
            }
            catch (Exception exception)
            {
                await workflowLifecycleOrchestrationService
                    .RecordFlowExecutionFailureAsync(
                        flowExecution: flowExecution,
                        exception: exception);
            }
            finally
            {
                if (!attemptConflict)
                {
                    await workflowLifecycleOrchestrationService
                        .FinishFlowExecutionAsync(flowExecution: flowExecution);
                }
            }
        });

    private static bool IsAttemptConflict(Exception exception)
    {
        while (exception is not null)
        {
            if (exception is WorkflowAttemptConflictException)
            {
                return true;
            }

            exception = exception.InnerException;
        }

        return false;
    }

    private static FlowExecution CreateFlowExecution(
        WorkflowRequest workflowRequest) =>
        new()
        {
            Request = workflowRequest
        };
}