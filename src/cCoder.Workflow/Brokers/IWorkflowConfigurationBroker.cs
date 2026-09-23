// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IWorkflowConfigurationBroker
{
    string GetServiceUrl();

    int GetSslPort();

    bool IsMigrating();

    TimeSpan GetInstanceMaintenanceMaxAge();

    TimeSpan GetExecutingInstanceTimeout();

    TimeSpan GetQueuePollingInterval();
}