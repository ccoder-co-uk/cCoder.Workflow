// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace Workflow.Brokers.WorkflowScriptExecutions;

internal interface IWorkflowScriptExecutionBroker
{
    Task<string> ExecuteScriptAsync(string payload, bool useDetails);
}