// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Services.Processings;

internal interface IWorkflowScriptExecutionProcessingService
{
    ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails);
}