// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

public class WorkflowConfiguration
{
    public string ConnectionString { get; set; }

    public bool DebugInfo { get; set; }

    public bool LogSQL { get; set; }

    public string RootPath { get; set; }

    public string ServiceUrl { get; set; }

    public int SslPort { get; set; }

    public WorkflowInstanceMaintenanceConfiguration InstanceMaintenance { get; set; }

    public WorkflowQueueInstanceManagementConfiguration QueueInstanceManagement { get; set; }

    public int ScheduledTaskPollingIntervalMilliseconds { get; set; }

    public bool IsMigrating { get; set; }

}