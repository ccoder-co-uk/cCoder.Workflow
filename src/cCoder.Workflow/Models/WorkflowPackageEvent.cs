// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Models;

public sealed class WorkflowPackageEvent
{
    public int AppId { get; set; }

    public Package Package { get; set; }
}