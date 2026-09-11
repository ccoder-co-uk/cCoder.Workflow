// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Services.Aggregations;

namespace cCoder.Workflow.Brokers.ServiceProviders;

internal sealed class WorkflowMigrationServiceProviderBroker(IServiceProvider serviceProvider)
    : IWorkflowMigrationServiceProviderBroker
{
    public T GetOperationService<T>(WorkflowMigrationOperation operation)
        where T : notnull =>
        serviceProvider.GetRequiredKeyedService<T>(serviceKey: operation);

    public void LogDebug(string message, params object[] args) =>
        GetOperationService<ILogger<WorkflowMigrationAggregationService>>(
            operation: WorkflowMigrationOperation.Logging)
                .LogDebug(message: message, args: args);
}