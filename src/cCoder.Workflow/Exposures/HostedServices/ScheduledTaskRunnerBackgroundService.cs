// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class ScheduledTaskRunnerBackgroundService(
    IServiceScopeBroker serviceScopeBroker)
    : BackgroundService, ICompositionExposure
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        serviceScopeBroker
            .RunScopedAsync<ITaskRunnerOrchestrationService>(
                operation: service =>
                    service.RunContinuouslyAsync(
                        cancellationToken: stoppingToken));
}