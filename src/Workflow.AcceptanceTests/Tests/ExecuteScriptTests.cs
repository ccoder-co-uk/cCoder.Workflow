// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteScriptTests
{
    private readonly Mock<IWorkflowFunctionsManager> processingServiceMock = new();
    private readonly ExecuteScript function;

    public ExecuteScriptTests() =>
        function = new ExecuteScript(
            workflowFunctionsProcessingService: processingServiceMock.Object);

    private static TestHttpRequestData CreateRequest(string payload)
    {
        return new TestHttpRequestData(body: payload);
    }
}