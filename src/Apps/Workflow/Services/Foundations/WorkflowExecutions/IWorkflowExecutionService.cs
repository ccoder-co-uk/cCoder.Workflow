// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace Workflow.Services.Foundations.WorkflowExecutions;

internal interface IWorkflowExecutionService
{
    Task RunWorkflowRequestAsync(string workflowRequestJson);
}