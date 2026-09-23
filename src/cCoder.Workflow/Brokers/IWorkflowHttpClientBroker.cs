// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IWorkflowHttpClientBroker
{
    ValueTask<string> PostTextAsync(
        string apiRoot,
        TimeSpan timeout,
        string requestUri,
        string content);
}