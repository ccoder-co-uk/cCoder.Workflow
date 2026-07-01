using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class QueueInstanceManagement(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IQueueInstanceManagement
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        await RunQueueInstanceManagementAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await RunQueueInstanceManagementAsync(stoppingToken);
    }

    private async Task RunQueueInstanceManagementAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IWorkflowInstanceManagementOrchestrationService workflowInstanceManagementOrchestrationService =
            scope.ServiceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();

        await workflowInstanceManagementOrchestrationService.RunQueueInstanceManagementAsync(stoppingToken);
    }
}
