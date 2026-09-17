// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;
using Workflow.Services.Foundations.WorkflowFunctions;
using Moq;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class HealthTests
{
    private readonly Mock<IWorkflowFunctionsService> processingServiceMock = new();
    private readonly Health function;

    public HealthTests() =>
        function = new Health(
            workflowFunctionsService: processingServiceMock.Object);
}