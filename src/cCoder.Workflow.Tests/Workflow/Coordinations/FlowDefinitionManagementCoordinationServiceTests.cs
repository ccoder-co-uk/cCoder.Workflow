// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Tests.Workflow.Coordinations;

public sealed partial class FlowDefinitionManagementCoordinationServiceTests
{
    private readonly Mock<IFlowDefinitionOrchestrationService>
        flowDefinitionOrchestrationServiceMock;

    private readonly Mock<IWorkflowInteractionOrchestrationService>
        workflowInteractionOrchestrationServiceMock;

    private readonly FlowDefinitionManagementCoordinationService
        flowDefinitionManagementCoordinationService;

    public FlowDefinitionManagementCoordinationServiceTests()
    {
        flowDefinitionOrchestrationServiceMock =
            new Mock<IFlowDefinitionOrchestrationService>(
                behavior: MockBehavior.Strict);

        workflowInteractionOrchestrationServiceMock =
            new Mock<IWorkflowInteractionOrchestrationService>(
                behavior: MockBehavior.Strict);

        flowDefinitionManagementCoordinationService =
            new FlowDefinitionManagementCoordinationService(
                flowDefinitionOrchestrationService:
                    flowDefinitionOrchestrationServiceMock.Object,
                interactionOrchestrationService:
                    workflowInteractionOrchestrationServiceMock.Object);
    }

    private static FlowDefinition CreateFlowDefinition() =>
        new()
        {
            Id = Guid.NewGuid()
        };
}