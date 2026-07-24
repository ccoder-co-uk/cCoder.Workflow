// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.ServiceProviders;

namespace cCoder.Workflow.Brokers.ServiceProviders;

internal interface IWorkflowMigrationServiceProviderBroker
{
    T GetOperationService<T>(WorkflowMigrationOperation operation)
        where T : notnull;
}