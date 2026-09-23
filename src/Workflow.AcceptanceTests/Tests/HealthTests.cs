// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;
using Workflow.Services.Foundations.WorkflowHttpResponses;
using Moq;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class HealthTests
{
    private readonly Mock<IWorkflowHttpResponseService> processingServiceMock =
        new();
    private readonly Health function;

    public HealthTests() =>
        function = new Health(
            workflowHttpResponseService: processingServiceMock.Object);
}