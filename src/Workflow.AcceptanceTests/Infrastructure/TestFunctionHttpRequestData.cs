using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Workflow.AcceptanceTests.Infrastructure;

internal sealed class TestFunctionContext : FunctionContext
{
    public override string InvocationId { get; } = Guid.NewGuid().ToString();
    public override string FunctionId { get; } = Guid.NewGuid().ToString();
    public override TraceContext TraceContext { get; } = null;
    public override BindingContext BindingContext { get; } = null;
    public override RetryContext RetryContext { get; } = null;
    public override IServiceProvider InstanceServices { get; set; } = new ServiceCollection().BuildServiceProvider();
    public override FunctionDefinition FunctionDefinition { get; } = null;
    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
    public override IInvocationFeatures Features { get; } = null;
    public override CancellationToken CancellationToken { get; } = CancellationToken.None;
}

internal sealed class TestHttpRequestData : HttpRequestData
{
    public TestHttpRequestData(string body = "")
        : base(new TestFunctionContext())
    {
        Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
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
