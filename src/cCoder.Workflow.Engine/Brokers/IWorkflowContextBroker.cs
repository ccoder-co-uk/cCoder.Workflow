// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Brokers;

internal interface IWorkflowContextBroker
{
    WorkflowExecutionContext CreateWorkflowExecutionContext(
        FlowExecution flowExecution);

    Task ExecuteWorkflowExecutionContextAsync(
        WorkflowExecutionContext workflowExecutionContext,
        string apiRoot,
        string authToken);
}