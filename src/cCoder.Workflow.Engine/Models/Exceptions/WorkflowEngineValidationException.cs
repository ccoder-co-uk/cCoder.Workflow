// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Models.Exceptions;

public sealed class WorkflowEngineValidationException(
    Exception innerException)
    : Exception(
        message: "Workflow engine validation failed.",
        innerException: innerException);