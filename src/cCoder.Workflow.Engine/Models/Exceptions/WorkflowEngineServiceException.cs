// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Models.Exceptions;

public sealed class WorkflowEngineServiceException : Exception
{
    public WorkflowEngineServiceException(Exception innerException)
        : base(
            message: "The workflow engine failed.",
            innerException: innerException)
    {
    }

    public WorkflowEngineServiceException(string message)
        : base(message: message)
    {
    }
}