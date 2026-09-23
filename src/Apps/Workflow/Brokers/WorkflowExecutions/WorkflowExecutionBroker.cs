// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;

namespace Workflow.Brokers.WorkflowExecutions;

internal sealed class WorkflowExecutionBroker(
    IFlowRunner flowRunner)
        : IWorkflowExecutionBroker
{
    public Task RunWorkflowRequestAsync(WorkflowRequest workflowRequest) =>
        flowRunner.RunAsync(workflowRequest: workflowRequest);
}