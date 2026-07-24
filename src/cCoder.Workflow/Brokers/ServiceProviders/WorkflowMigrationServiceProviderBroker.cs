// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.ServiceProviders;

namespace cCoder.Workflow.Brokers.ServiceProviders;

internal sealed class WorkflowMigrationServiceProviderBroker(IServiceProvider serviceProvider)
    : IWorkflowMigrationServiceProviderBroker
{
    public T GetOperationService<T>(WorkflowMigrationOperation operation)
        where T : notnull =>
        serviceProvider.GetRequiredKeyedService<T>(serviceKey: operation);
}