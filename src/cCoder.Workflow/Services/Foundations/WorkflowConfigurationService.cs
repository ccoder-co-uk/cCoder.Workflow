// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowConfigurationService(
    IWorkflowConfigurationBroker configurationBroker)
    : IWorkflowConfigurationService
{
    public string GetServiceUrl() =>
        TryCatch(operation: () =>
        {
            return configurationBroker.GetServiceUrl();
        });

    public int GetSslPort() =>
        TryCatch(operation: () => configurationBroker.GetSslPort());

    public bool IsMigrating() =>
        TryCatch(operation: () => configurationBroker.IsMigrating());

    public TimeSpan GetInstanceMaintenanceMaxAge() =>
        TryCatch(operation: () => configurationBroker.GetInstanceMaintenanceMaxAge());

    public TimeSpan GetExecutingInstanceTimeout() =>
        TryCatch(operation: () => configurationBroker.GetExecutingInstanceTimeout());

    public TimeSpan GetQueuePollingInterval() =>
        TryCatch(operation: () => configurationBroker.GetQueuePollingInterval());
}