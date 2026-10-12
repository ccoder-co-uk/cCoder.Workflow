// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Models;

public sealed class WorkflowInstanceExchange
{
    public FlowInstanceData Expected { get; set; }

    public FlowInstanceData Replacement { get; set; }
}