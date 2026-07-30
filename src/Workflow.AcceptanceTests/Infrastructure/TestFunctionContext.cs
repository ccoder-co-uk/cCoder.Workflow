// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Workflow.AcceptanceTests.Infrastructure;

internal sealed class TestFunctionContext : FunctionContext
{
    public override string InvocationId { get; } = Guid.NewGuid()
        .ToString();
    public override string FunctionId { get; } = Guid.NewGuid()
        .ToString();
    public override TraceContext TraceContext { get; } = null;
    public override BindingContext BindingContext { get; } = null;
    public override RetryContext RetryContext { get; } = null;
    public override IServiceProvider InstanceServices { get; set; } =
        new ServiceCollection().BuildServiceProvider();
    public override FunctionDefinition FunctionDefinition { get; } = null;
    public override IDictionary<object, object> Items { get; set; } =
        new Dictionary<object, object>();
    public override IInvocationFeatures Features { get; } = null;
    public override CancellationToken CancellationToken { get; } =
        CancellationToken.None;
}