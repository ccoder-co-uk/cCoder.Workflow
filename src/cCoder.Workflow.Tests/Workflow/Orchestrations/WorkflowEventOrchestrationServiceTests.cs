// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class WorkflowEventOrchestrationServiceTests
{
    private readonly Mock<IWorkflowEventProcessingService> workflowEventProcessingServiceMock;
    private readonly Mock<IWorkflowEventEventProcessingService> workflowEventEventProcessingServiceMock;
    private readonly WorkflowEventOrchestrationService orchestrationService;

    public WorkflowEventOrchestrationServiceTests()
    {
        workflowEventProcessingServiceMock = new Mock<IWorkflowEventProcessingService>(MockBehavior.Strict);
        workflowEventEventProcessingServiceMock = new Mock<IWorkflowEventEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new WorkflowEventOrchestrationService(
            workflowEventProcessingServiceMock.Object,
            workflowEventEventProcessingServiceMock.Object
        );
    }

    private static WorkflowEvent CreateRandomWorkflowEvent() =>
        Builder<WorkflowEvent>.CreateNew()
            .Build();
}