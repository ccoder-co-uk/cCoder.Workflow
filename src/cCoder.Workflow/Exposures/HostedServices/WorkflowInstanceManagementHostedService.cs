using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace cCoder.Workflow.Exposures.HostedServices;

public sealed class WorkflowInstanceManagementHostedService(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IWorkflowInstanceManagementOrchestrationService orchestrationService =
                    scope.ServiceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();
                await orchestrationService.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                ILogger<WorkflowInstanceManagementHostedService> log =
                    scope.ServiceProvider.GetRequiredService<ILogger<WorkflowInstanceManagementHostedService>>();
                log.LogError(ex, ex.Message);
            }
        }
    }
}
