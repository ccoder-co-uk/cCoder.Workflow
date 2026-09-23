// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowConfigurationBroker(
    WorkflowConfiguration configuration)
    : IWorkflowConfigurationBroker
{
    public string GetServiceUrl() =>
        configuration.ServiceUrl;

    public int GetSslPort() =>
        configuration.SslPort;

    public bool IsMigrating() =>
        configuration.IsMigrating;

    public TimeSpan GetInstanceMaintenanceMaxAge() =>
        TimeSpan.FromDays(value: configuration.InstanceMaintenance.MaxAgeDays);

    public TimeSpan GetExecutingInstanceTimeout() =>
        TimeSpan.FromMinutes(
            value: configuration.QueueInstanceManagement.ExecutingTimeoutMinutes);

    public TimeSpan GetQueuePollingInterval() =>
        TimeSpan.FromMilliseconds(
            milliseconds: configuration.QueueInstanceManagement.PollingIntervalMilliseconds);
}