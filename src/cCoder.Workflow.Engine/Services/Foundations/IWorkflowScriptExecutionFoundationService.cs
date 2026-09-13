// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Services.Foundations;

internal interface IWorkflowScriptExecutionFoundationService
{
    ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails);
}