// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Workflow.Activities.Models;

public sealed class WorkflowAttemptConflictException : InvalidOperationException
{
    public WorkflowAttemptConflictException() : base(message: "Workflow execution attempt no longer owns this operation.") { }
}