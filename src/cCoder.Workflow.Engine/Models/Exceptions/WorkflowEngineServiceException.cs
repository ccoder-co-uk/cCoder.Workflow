// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Models.Exceptions;

public sealed class WorkflowEngineServiceException(
    Exception innerException)
    : Exception(
        message: "The workflow engine failed.",
        innerException: innerException);