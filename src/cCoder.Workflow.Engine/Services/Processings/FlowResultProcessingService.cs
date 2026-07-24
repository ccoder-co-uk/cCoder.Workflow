// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Support;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowResultProcessingService
    : IFlowResultProcessingService
{
    public ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(
                inputs:
                [
                    flowInstanceData,
                    apiRoot,
                    authToken
                ]);

            using HttpClient api =
                CreateHttpClient(apiRoot: apiRoot);

            api.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: authToken);

            string payload = JsonConvert.SerializeObject(
                value: new
                {
                    flowInstanceData.Id,
                    flowInstanceData.FlowDefinitionId,
                    flowInstanceData.Name,
                    flowInstanceData.State,
                    flowInstanceData.ReportingComponentName,
                    flowInstanceData.Caller,
                    flowInstanceData.ContextString,
                    flowInstanceData.Start,
                    flowInstanceData.End
                },
                formatting: Formatting.None);

            using HttpResponseMessage response = await api.PutAsync(
                requestUri:
                    $"Workflow/FlowInstanceData"
                    + $"({flowInstanceData.Id})",
                content: new StringContent(
                    content: payload,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                string responseBody =
                    await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Workflow result save failed with status "
                    + $"{(int)response.StatusCode} "
                    + $"({response.StatusCode})."
                    + $"{Environment.NewLine}Payload:"
                    + $"{Environment.NewLine}{payload}"
                    + $"{Environment.NewLine}Response:"
                    + $"{Environment.NewLine}{responseBody}");
            }

            response.EnsureSuccessStatusCode();
        });

    private static HttpClient CreateHttpClient(
        string apiRoot) =>
        new(new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip
                | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback =
                CertChainValidator.ValidateCertChain
        })
        {
            BaseAddress = new Uri(apiRoot)
        };
}