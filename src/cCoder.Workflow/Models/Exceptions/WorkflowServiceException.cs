// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Workflow.Models.Exceptions;

public sealed class WorkflowServiceException(Exception innerException)
    : Exception(
        message: "The Workflow service failed.",
        innerException: innerException);