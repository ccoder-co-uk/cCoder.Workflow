// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class WorkflowContextBroker
    : IWorkflowContextBroker
{
    public WorkflowExecutionContext CreateWorkflowExecutionContext(
        FlowExecution flowExecution) =>
        new(flowExecution: flowExecution);

    public Task ExecuteWorkflowExecutionContextAsync(
        WorkflowExecutionContext workflowExecutionContext,
        string apiRoot,
        string authToken) =>
        workflowExecutionContext.ExecuteAsync(
            apiRoot: apiRoot,
            authToken: authToken);
}