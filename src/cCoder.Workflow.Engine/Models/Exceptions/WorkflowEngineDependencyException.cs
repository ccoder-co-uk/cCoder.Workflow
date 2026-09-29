// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Workflow.Engine.Models.Exceptions;

public sealed class WorkflowEngineDependencyException(
    Exception innerException)
    : Exception(
        message: "A workflow engine dependency failed.",
        innerException: innerException);