// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Exposures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Dependencies.HostedServices;

internal sealed class QueueInstanceBackgroundServiceDependency(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IQueueInstanceBackgroundServiceDependency
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IWorkflowInstanceManager workflowInstanceProcessingService =
            scope.ServiceProvider.GetRequiredService<IWorkflowInstanceManager>();

        await workflowInstanceProcessingService.RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(cancellationToken: stoppingToken);
    }
}