// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Workflow.Engine.Services.Processings;

internal interface IWorkflowScriptExecutionProcessingService
{
    ValueTask<string> ExecuteWorkflowScriptAsync(
        string payload,
        bool useDetails);
}