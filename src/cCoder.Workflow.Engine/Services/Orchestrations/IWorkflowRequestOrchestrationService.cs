// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public interface IWorkflowRequestOrchestrationService
{
    ValueTask ExecuteWorkflowRequestAsync(
        WorkflowRequest workflowRequest);
}