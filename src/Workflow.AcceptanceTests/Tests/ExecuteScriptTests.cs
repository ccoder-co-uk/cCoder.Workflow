// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;
using Workflow.Services.Orchestrations.WorkflowScriptFunctions;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteScriptTests
{
    private readonly Mock<IWorkflowScriptFunctionsOrchestrationService>
        processingServiceMock = new();
    private readonly ExecuteScript function;

    public ExecuteScriptTests() =>
        function = new ExecuteScript(
            workflowScriptFunctionsOrchestrationService:
                processingServiceMock.Object);

    private static TestHttpRequestData CreateRequest(string payload)
    {
        return new TestHttpRequestData(body: payload);
    }
}