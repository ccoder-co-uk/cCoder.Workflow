// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

public sealed class WorkflowQueueInstanceManagementConfiguration
{
    public WorkflowQueueInstanceManagementConfiguration()
    {
        ExecutingTimeoutMinutes = 30;
        PollingIntervalMilliseconds = 60000;
    }

    public double ExecutingTimeoutMinutes { get; set; }
    public int PollingIntervalMilliseconds { get; set; }
}