// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using Moq;
using Newtonsoft.Json;
using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;
using Workflow.Services.Processings.WorkflowFunctions;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteTests
{
    private readonly Mock<IWorkflowFunctionsProcessingService> processingServiceMock = new();
    private readonly Execute function;

    public ExecuteTests() =>
        function = new Execute(
            workflowFunctionsProcessingService: processingServiceMock.Object);

    private static TestHttpRequestData CreateRequest(WorkflowRequest request) =>
        new(JsonConvert.SerializeObject(value: request));
}