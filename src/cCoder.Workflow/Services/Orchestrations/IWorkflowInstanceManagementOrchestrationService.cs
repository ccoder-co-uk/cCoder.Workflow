namespace cCoder.Workflow.Services.Orchestrations;

public interface IWorkflowInstanceManagementOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);
    object[] GetStats();
    ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid id);
}
