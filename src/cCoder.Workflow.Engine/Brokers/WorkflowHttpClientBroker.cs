// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class WorkflowHttpClientBroker
    : IWorkflowHttpClientBroker
{
    public async ValueTask<string> GetStringAsync(
        string apiRoot,
        string authToken,
        string requestUri)
    {
        using WorkflowHttpClientDependency dependency =
            new(apiRoot: apiRoot, authToken: authToken);

        return await dependency.GetStringAsync(
            requestUri: requestUri);
    }

    public async ValueTask<WorkflowHttpResult> PutJsonAsync(
        string apiRoot,
        string authToken,
        string requestUri,
        string payload)
    {
        using WorkflowHttpClientDependency dependency =
            new(apiRoot: apiRoot, authToken: authToken);

        (bool isSuccess, int statusCode, string status, string body) =
            await dependency.PutJsonAsync(
            requestUri: requestUri,
            payload: payload);

        return new WorkflowHttpResult
        {
            IsSuccess = isSuccess,
            StatusCode = statusCode,
            Status = status,
            Body = body
        };
    }
}