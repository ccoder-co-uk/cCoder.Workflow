// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public interface IWorkflowScriptExecutionOrchestrationService
{
    Task<string> ExecuteAsync(string payload, bool useDetails);
}