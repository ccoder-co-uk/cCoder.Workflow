// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace Workflow.Brokers.WorkflowExecutions;

internal interface IWorkflowExecutionBroker
{
    Task RunWorkflowRequestAsync(WorkflowRequest workflowRequest);
}