// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Workflow.Activities.Models;

public sealed class WorkflowAttemptIdentity
{
    public Guid? ExecutionAttemptId { get; set; }

    public Guid? ExecutionOwnerId { get; set; }
}