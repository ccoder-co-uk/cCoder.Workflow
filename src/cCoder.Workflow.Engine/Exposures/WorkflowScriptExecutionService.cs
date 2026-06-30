using cCoder.Workflow.Engine.Services.Orchestrations;

namespace cCoder.Workflow.Engine.Exposures;

public sealed class WorkflowScriptExecutionService(
    IWorkflowScriptExecutionOrchestrationService workflowScriptExecutionOrchestrationService)
    : IWorkflowScriptExecutionService
{
    public Task<string> ExecuteAsync(string payload, bool useDetails) =>
        workflowScriptExecutionOrchestrationService.ExecuteAsync(payload, useDetails);
}
