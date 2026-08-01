// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

public sealed class WorkflowInstanceMaintenanceConfiguration
{
    public WorkflowInstanceMaintenanceConfiguration() =>
        MaxAgeDays = 7;

    public double MaxAgeDays { get; set; }
}