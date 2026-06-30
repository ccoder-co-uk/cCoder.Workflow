namespace cCoder.Workflow.Engine.Services.Orchestrations;

public interface IWorkflowScriptExecutionOrchestrationService
{
    Task<string> ExecuteAsync(string payload, bool useDetails);
}
