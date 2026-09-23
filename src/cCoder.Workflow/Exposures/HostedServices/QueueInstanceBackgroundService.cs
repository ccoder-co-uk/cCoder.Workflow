// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Aggregations;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class QueueInstanceBackgroundService(
    IServiceScopeBroker serviceScopeBroker)
    : BackgroundService, ICompositionExposure
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        serviceScopeBroker
            .RunScopedAsync<IWorkflowInstanceAggregationService>(
            operation: manager =>
                manager
                    .RunQueueInstanceBackgroundServiceDependencyContinuouslyAsync(
                        cancellationToken: stoppingToken));
}