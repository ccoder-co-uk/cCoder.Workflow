// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.AcceptanceTests.Infrastructure;

internal sealed class TestHttpResponseData(FunctionContext functionContext)
    : HttpResponseData(functionContext)
{
    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; } = [];
    public override Stream Body { get; set; } = new MemoryStream();
    public override HttpCookies Cookies { get; } = null;

    public string ReadBody()
    {
        Body.Position = 0;
        using StreamReader reader = new(Body, leaveOpen: true);
        return reader.ReadToEnd();
    }
}