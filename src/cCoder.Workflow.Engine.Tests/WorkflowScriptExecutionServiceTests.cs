// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionServiceTests
{
    private readonly Mock<IWorkflowScriptExecutionProcessingService> processingServiceMock = new();
    private readonly WorkflowScriptExecutionService workflowScriptExecutionService;

    public WorkflowScriptExecutionServiceTests() =>
        workflowScriptExecutionService = new WorkflowScriptExecutionService(
            workflowScriptExecutionProcessingService: processingServiceMock.Object);
}