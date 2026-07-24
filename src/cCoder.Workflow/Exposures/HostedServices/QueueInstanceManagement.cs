// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class QueueInstanceManagement(IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IQueueInstanceManagement
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IWorkflowInstanceManagementOrchestrationService workflowInstanceManagementOrchestrationService =
            scope.ServiceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();

        await workflowInstanceManagementOrchestrationService.RunQueueInstanceManagementContinuouslyAsync(stoppingToken);
    }
}