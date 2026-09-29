// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowScriptExecutionService
{
    ValueTask<string> ExecuteAsync(
        string serviceUrl,
        string script);
}