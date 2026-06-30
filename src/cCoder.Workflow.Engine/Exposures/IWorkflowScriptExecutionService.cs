namespace cCoder.Workflow.Engine.Exposures;

public interface IWorkflowScriptExecutionService
{
    Task<string> ExecuteAsync(string payload, bool useDetails);
}
