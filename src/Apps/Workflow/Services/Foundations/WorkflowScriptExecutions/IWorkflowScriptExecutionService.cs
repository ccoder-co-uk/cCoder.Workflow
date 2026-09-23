// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.Services.Foundations.WorkflowScriptExecutions;

internal interface IWorkflowScriptExecutionService
{
    Task<string> ExecuteScriptAsync(string payload, bool useDetails);
}