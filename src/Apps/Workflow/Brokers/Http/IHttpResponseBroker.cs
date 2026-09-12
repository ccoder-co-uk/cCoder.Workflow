// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Workflow.Brokers.Http;

internal interface IHttpResponseBroker
{
    HttpResponseData CreateResponse(
        HttpRequestData request,
        HttpStatusCode statusCode);

    Task WriteStringAsync(
        HttpResponseData response,
        string value);
}