// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using cCoder.Workflow.Models;

namespace cCoder.Workflow.Dependencies;

internal sealed class WorkflowHttpClientDependency : HttpClient
{
    internal WorkflowHttpClientDependency(
        string apiRoot,
        TimeSpan? timeout = null)
        : base(handler: new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
    {
        BaseAddress = new Uri(apiRoot);

        if (timeout is not null)
        {
            Timeout = timeout.Value;
        }
    }

    internal async ValueTask<string> PostTextAsync(
        string requestUri,
        string content)
    {
        using HttpResponseMessage response = await PostAsync(
            requestUri: requestUri,
            content: new StringContent(
                content: content,
                encoding: Encoding.UTF8,
                mediaType: "text/plain"));

        return await response.Content.ReadAsStringAsync();
    }

    internal async ValueTask<WorkflowHttpResult> PostJsonAsync(
        string requestUri,
        string content)
    {
        using HttpResponseMessage response = await PostAsync(
            requestUri: requestUri,
            content: new StringContent(
                content: content,
                encoding: Encoding.UTF8,
                mediaType: "application/json"));

        return new WorkflowHttpResult
        {
            IsSuccess = response.IsSuccessStatusCode,
            Body = await response.Content.ReadAsStringAsync()
        };
    }
}