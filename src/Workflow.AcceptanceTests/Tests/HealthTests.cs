// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.AcceptanceTests.Infrastructure;
using Workflow.Exposures;
using Moq;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class HealthTests
{
    private readonly Mock<IWorkflowFunctionsManager> processingServiceMock = new();
    private readonly Health function;

    public HealthTests() =>
        function = new Health(
            workflowFunctionsProcessingService: processingServiceMock.Object);
}