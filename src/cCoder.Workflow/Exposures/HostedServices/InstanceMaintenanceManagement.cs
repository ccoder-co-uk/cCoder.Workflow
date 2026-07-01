using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class InstanceMaintenanceManagement(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IInstanceMaintenanceManagement
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        await RunInstanceMaintenanceAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await RunInstanceMaintenanceAsync(stoppingToken);
    }

    private async Task RunInstanceMaintenanceAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IWorkflowInstanceManagementOrchestrationService workflowInstanceManagementOrchestrationService =
            scope.ServiceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();

        await workflowInstanceManagementOrchestrationService.RunInstanceMaintenanceAsync(stoppingToken);
    }
}
