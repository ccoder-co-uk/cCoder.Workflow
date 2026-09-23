// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.Services.Foundations.WorkflowFunctionStreams;

internal interface IWorkflowFunctionStreamService
{
    ValueTask<string> ReadHttpRequestDataBodyAsync(HttpRequestData request);
}