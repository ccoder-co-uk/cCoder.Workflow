// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Models;

internal sealed class WorkflowHttpResult
{
    internal bool IsSuccess { get; init; }

    internal int StatusCode { get; init; }

    internal string Status { get; init; }

    internal string Body { get; init; }
}