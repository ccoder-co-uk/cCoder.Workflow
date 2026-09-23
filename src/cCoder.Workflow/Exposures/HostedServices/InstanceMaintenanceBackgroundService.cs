// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Processings;
using Microsoft.Extensions.Hosting;

namespace cCoder.Workflow.Exposures.HostedServices;

internal sealed class InstanceMaintenanceBackgroundService(
    IServiceScopeBroker serviceScopeBroker)
    : BackgroundService, ICompositionExposure
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        serviceScopeBroker
            .RunScopedAsync<IWorkflowInstanceProcessingService>(
            operation: manager =>
                manager.RunInstanceMaintenanceContinuouslyAsync(
                    cancellationToken: stoppingToken));
}