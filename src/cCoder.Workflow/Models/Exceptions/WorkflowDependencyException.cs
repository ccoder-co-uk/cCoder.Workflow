// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.Exceptions;

public sealed class WorkflowDependencyException(Exception innerException)
    : Exception(
        message: "A Workflow dependency failed.",
        innerException: innerException);