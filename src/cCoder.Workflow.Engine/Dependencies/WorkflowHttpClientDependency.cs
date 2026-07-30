// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Models;
using System.Text;

namespace cCoder.Workflow.Engine.Dependencies;

internal sealed class WorkflowHttpClientDependency : HttpClient
{
    internal WorkflowHttpClientDependency(
        string apiRoot,
        string authToken = null)
        : base(handler: new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback =
                CertChainValidator.ValidateCertChain
        })
    {
        BaseAddress = new Uri(apiRoot);

        if (!string.IsNullOrWhiteSpace(value: authToken))
        {
            DefaultRequestHeaders.Authorization =
                new(
                    scheme: "Bearer",
                    parameter: authToken);
        }
    }

    internal async ValueTask<WorkflowHttpResult> PutJsonAsync(
        string requestUri,
        string payload)
    {
        using HttpResponseMessage response = await PutAsync(
            requestUri: requestUri,
            content: new StringContent(
                content: payload,
                encoding: Encoding.UTF8,
                mediaType: "application/json"));

        return new WorkflowHttpResult
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            Status = response.StatusCode.ToString(),
            Body = await response.Content.ReadAsStringAsync()
        };
    }
}