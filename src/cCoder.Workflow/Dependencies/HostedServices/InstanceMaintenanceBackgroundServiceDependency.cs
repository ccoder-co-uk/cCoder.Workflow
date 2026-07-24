// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Processings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Dependencies.HostedServices;

internal sealed class InstanceMaintenanceBackgroundServiceDependency(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IInstanceMaintenanceBackgroundServiceDependency
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IWorkflowInstanceProcessingService workflowInstanceProcessingService =
            scope.ServiceProvider.GetRequiredService<IWorkflowInstanceProcessingService>();

        await workflowInstanceProcessingService.RunInstanceMaintenanceContinuouslyAsync(cancellationToken: stoppingToken);
    }
}