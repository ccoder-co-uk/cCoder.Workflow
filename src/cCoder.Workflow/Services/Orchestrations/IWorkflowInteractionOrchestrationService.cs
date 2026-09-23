// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

internal interface IWorkflowInteractionOrchestrationService
{
    ValueTask<string> ExecuteScriptAsync(string script);

    ValueTask<string> ReadRequestBodyAsync(Stream stream);
}