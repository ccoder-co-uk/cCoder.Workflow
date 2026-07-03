namespace cCoder.Workflow.Services.Orchestrations;

public interface ITaskRunnerOrchestrationService
{
    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);

    Task RunAsync(CancellationToken cancellationToken = default);
}
