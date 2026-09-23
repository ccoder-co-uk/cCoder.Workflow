// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.Brokers.WorkflowFunctionStreams;

internal interface IWorkflowFunctionStreamBroker
{
    ValueTask<string> ReadBodyAsync(HttpRequestData request);
}