// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Workflow.Engine.Services.Processings;

namespace cCoder.Workflow.Engine.Exposures;

internal sealed class WorkflowScriptExecutionService(
    IWorkflowScriptExecutionProcessingService
        workflowScriptExecutionProcessingService)
    : IWorkflowScriptExecutionService
{
    public Task<string> ExecuteAsync(string payload, bool useDetails) =>
        workflowScriptExecutionProcessingService
            .ExecuteWorkflowScriptAsync(
                payload: payload,
                useDetails: useDetails)
            .AsTask();
}