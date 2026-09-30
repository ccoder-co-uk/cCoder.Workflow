// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowHttpClientBroker(
    IHttpClientFactory httpClientFactory)
    : IWorkflowHttpClientBroker
{
    public ValueTask<string> PostTextAsync(
        string apiRoot,
        TimeSpan timeout,
        string requestUri,
        string content)
    {
        using HttpClient httpClient = httpClientFactory.CreateClient(
            name: nameof(WorkflowHttpClientBroker));

        httpClient.BaseAddress = new Uri(uriString: apiRoot);
        httpClient.Timeout = timeout;

        return PostTextAsync(
            httpClient: httpClient,
            requestUri: requestUri,
            content: content);
    }

    private static async ValueTask<string> PostTextAsync(
        HttpClient httpClient,
        string requestUri,
        string content)
    {
        using StringContent requestContent = new(
            content: content,
            encoding: Encoding.UTF8,
            mediaType: "text/plain");

        using HttpResponseMessage response = await httpClient.PostAsync(
            requestUri: requestUri,
            content: requestContent);

        return await response.Content.ReadAsStringAsync();
    }
}