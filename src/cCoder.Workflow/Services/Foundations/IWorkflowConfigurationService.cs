// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowConfigurationService
{
    string GetServiceUrl();

    int GetSslPort();

    bool IsMigrating();

    TimeSpan GetInstanceMaintenanceMaxAge();

    TimeSpan GetExecutingInstanceTimeout();

    TimeSpan GetQueuePollingInterval();
}