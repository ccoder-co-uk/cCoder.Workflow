// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.Exceptions;

public sealed class WorkflowValidationException(Exception innerException)
    : Exception(
        message: "Workflow validation failed.",
        innerException: innerException);