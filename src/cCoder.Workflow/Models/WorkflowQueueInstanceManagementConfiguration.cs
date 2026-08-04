// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

public sealed class WorkflowQueueInstanceManagementConfiguration
{
    public double ExecutingTimeoutMinutes { get; set; }
    public int PollingIntervalMilliseconds { get; set; }
}