// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Dependencies.HostedServices;

internal sealed class ScheduledTaskRunnerBackgroundServiceDependency(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IScheduledTaskRunnerBackgroundServiceDependency
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        ITaskRunnerOrchestrationService taskRunnerOrchestrationService =
            scope.ServiceProvider.GetRequiredService<ITaskRunnerOrchestrationService>();

        await taskRunnerOrchestrationService.RunContinuouslyAsync(cancellationToken: stoppingToken);
    }
}