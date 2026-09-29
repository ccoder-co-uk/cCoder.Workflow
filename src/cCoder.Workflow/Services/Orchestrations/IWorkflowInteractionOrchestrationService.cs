// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace cCoder.Workflow.Services.Orchestrations;

internal interface IWorkflowInteractionOrchestrationService
{
    ValueTask<string> ExecuteScriptAsync(string script);

    ValueTask<string> ReadRequestBodyAsync(Stream stream);
}