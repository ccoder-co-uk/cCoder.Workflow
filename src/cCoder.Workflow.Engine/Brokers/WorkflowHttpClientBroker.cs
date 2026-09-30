// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class WorkflowHttpClientBroker(
    IHttpClientFactory httpClientFactory)
    : IWorkflowHttpClientBroker
{
    public async ValueTask<string> GetStringAsync(
        string apiRoot,
        string authToken,
        string requestUri)
    {
        using HttpClient httpClient = CreateHttpClient(
            apiRoot: apiRoot,
            authToken: authToken);

        return await httpClient.GetStringAsync(
            requestUri: requestUri);
    }

    public async ValueTask<WorkflowHttpResult> PutJsonAsync(
        string apiRoot,
        string authToken,
        string requestUri,
        string payload)
    {
        using HttpClient httpClient = CreateHttpClient(
            apiRoot: apiRoot,
            authToken: authToken);

        using StringContent requestContent = new(
            content: payload,
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        using HttpResponseMessage response = await httpClient.PutAsync(
            requestUri: requestUri,
            content: requestContent);

        return new WorkflowHttpResult
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Status = response.StatusCode.ToString(),
            Body = await response.Content.ReadAsStringAsync()
        };
    }

    private HttpClient CreateHttpClient(
        string apiRoot,
        string authToken)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(
            name: nameof(WorkflowHttpClientBroker));

        httpClient.BaseAddress = new Uri(uriString: apiRoot);

        AuthenticationHeaderValue.TryParse(
            input: $"Bearer {authToken}",
            parsedValue: out AuthenticationHeaderValue authorization);

        httpClient.DefaultRequestHeaders.Authorization = authorization;

        return httpClient;
    }
}