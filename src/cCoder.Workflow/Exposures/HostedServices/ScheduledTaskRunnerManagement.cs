// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class ScheduledTaskRunnerManagement(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IScheduledTaskRunnerManagement
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        ITaskRunnerOrchestrationService taskRunnerOrchestrationService =
            scope.ServiceProvider.GetRequiredService<ITaskRunnerOrchestrationService>();

        await taskRunnerOrchestrationService.RunContinuouslyAsync(stoppingToken);
    }
}