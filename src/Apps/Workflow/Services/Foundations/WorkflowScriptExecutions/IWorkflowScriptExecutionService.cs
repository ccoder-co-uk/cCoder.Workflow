// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace Workflow.Services.Foundations.WorkflowScriptExecutions;

internal interface IWorkflowScriptExecutionService
{
    Task<string> ExecuteScriptAsync(string payload, bool useDetails);
}