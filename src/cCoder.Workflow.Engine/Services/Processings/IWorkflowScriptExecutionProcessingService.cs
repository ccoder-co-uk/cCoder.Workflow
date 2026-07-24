// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Services.Processings;

public interface IWorkflowScriptExecutionProcessingService
{
    ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails);
}