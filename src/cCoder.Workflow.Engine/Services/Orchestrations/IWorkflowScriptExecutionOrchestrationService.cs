// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal interface IWorkflowScriptExecutionOrchestrationService
{
    Task<string> ExecuteAsync(
        string payload,
        bool useDetails);
}