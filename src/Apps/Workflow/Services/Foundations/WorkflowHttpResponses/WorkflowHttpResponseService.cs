// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Workflow.Brokers.Http;

namespace Workflow.Services.Foundations.WorkflowHttpResponses;

internal sealed partial class WorkflowHttpResponseService(
    IHttpResponseBroker httpResponseBroker)
        : IWorkflowHttpResponseService
{
    public Task<HttpResponseData> CreateHttpResponseDataAsync(
        HttpRequestData request,
        string content) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request, content]);

            HttpResponseData response = httpResponseBroker.CreateResponse(
                request: request,
                statusCode: HttpStatusCode.OK);

            await httpResponseBroker.WriteStringAsync(
                response: response,
                value: content);

            return response;
        });
}