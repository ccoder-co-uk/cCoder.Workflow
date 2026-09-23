// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using cCoder.Workflow.Activities.Support;

namespace cCoder.Workflow.Engine.Dependencies;

internal sealed class WorkflowHttpClientDependency : IDisposable
{
    private readonly HttpClient client;

    internal WorkflowHttpClientDependency(
        string apiRoot,
        string authToken = null)
    {
        client = new HttpClient(handler: new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback =
                CertChainValidator.ValidateCertChain
        })
        {
            BaseAddress = new Uri(apiRoot)
        };

        if (!string.IsNullOrWhiteSpace(value: authToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: authToken);
        }
    }

    internal ValueTask<string> GetStringAsync(string requestUri) =>
        new(client.GetStringAsync(requestUri: requestUri));

    internal async ValueTask<(bool IsSuccess, int StatusCode, string Status, string Body)> PutJsonAsync(
        string requestUri,
        string payload)
    {
        using HttpResponseMessage response = await client.PutAsync(
            requestUri: requestUri,
            content: new StringContent(
                content: payload,
                encoding: Encoding.UTF8,
                mediaType: "application/json"));

        return (
            IsSuccess: response.IsSuccessStatusCode,
            StatusCode: (int)response.StatusCode,
            Status: response.StatusCode.ToString(),
            Body: await response.Content.ReadAsStringAsync());
    }

    public void Dispose() =>
        client.Dispose();
}