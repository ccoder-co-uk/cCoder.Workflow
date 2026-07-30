// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Workflow.AcceptanceTests.Infrastructure;

internal sealed class TestHttpRequestData : HttpRequestData
{
    public TestHttpRequestData(string body = "")
        : base(new TestFunctionContext())
    {
        Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(s: body));
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; } = [];
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = [];
    public override Uri Url { get; } = new("https://localhost/api/test");
    public override IEnumerable<ClaimsIdentity> Identities { get; } = [];
    public override string Method { get; } = "POST";

    public override HttpResponseData CreateResponse() =>
        new TestHttpResponseData(FunctionContext);
}