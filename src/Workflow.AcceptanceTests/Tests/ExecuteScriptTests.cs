// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using Moq;
using Workflow.AcceptanceTests.Infrastructure;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteScriptTests
{
    private readonly Mock<IWorkflowScriptExecutionService> scriptExecutionServiceMock = new();
    private readonly ExecuteScript function;

    public ExecuteScriptTests() =>
        function = new ExecuteScript(scriptExecutionServiceMock.Object);

    private static TestHttpRequestData CreateRequest(string payload) =>
        new(payload);
}