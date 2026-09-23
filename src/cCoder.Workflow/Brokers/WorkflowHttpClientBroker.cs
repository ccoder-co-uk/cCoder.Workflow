// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowHttpClientBroker(
    WorkflowHttpClientDependency httpClientDependency)
    : IWorkflowHttpClientBroker
{
    public ValueTask<string> PostTextAsync(
        string apiRoot,
        TimeSpan timeout,
        string requestUri,
        string content) =>
        httpClientDependency.PostTextAsync(
            apiRoot: apiRoot,
            timeout: timeout,
            requestUri: requestUri,
            content: content);
}