// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.Brokers.WorkflowScriptExecutions;

internal interface IWorkflowScriptExecutionBroker
{
    Task<string> ExecuteScriptAsync(string payload, bool useDetails);
}