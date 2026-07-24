// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class FlowDefinitionCoordinationServiceTests
{
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly Mock<IFlowInstanceDataOrchestrationService> flowInstanceDataOrchestrationServiceMock;
    private readonly FlowDefinitionCoordinationService coordinationService;

    public FlowDefinitionCoordinationServiceTests()
    {
        flowDefinitionOrchestrationServiceMock =
            new Mock<IFlowDefinitionOrchestrationService>(MockBehavior.Strict);
        flowInstanceDataOrchestrationServiceMock =
            new Mock<IFlowInstanceDataOrchestrationService>(MockBehavior.Strict);
        coordinationService = new FlowDefinitionCoordinationService(
            flowDefinitionOrchestrationServiceMock.Object,
            flowInstanceDataOrchestrationServiceMock.Object);
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>
            .CreateNew()
            .With(func: flow =>
                flow.Instances = [Builder<FlowInstanceData>.CreateNew()
            .Build()]
            )
            .Build();
}