// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Services.Coordinations;

internal interface IWorkflowRequestCoordinationService
{
    ValueTask ExecuteWorkflowRequestAsync(
        WorkflowRequest workflowRequest);
}