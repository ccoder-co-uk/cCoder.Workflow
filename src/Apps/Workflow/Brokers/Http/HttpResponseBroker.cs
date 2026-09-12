// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Workflow.Brokers.Http;

internal sealed class HttpResponseBroker : IHttpResponseBroker
{
    public HttpResponseData CreateResponse(
        HttpRequestData request,
        HttpStatusCode statusCode) =>
        request.CreateResponse(statusCode: statusCode);

    public Task WriteStringAsync(
        HttpResponseData response,
        string value) =>
        response.WriteStringAsync(value: value);
}