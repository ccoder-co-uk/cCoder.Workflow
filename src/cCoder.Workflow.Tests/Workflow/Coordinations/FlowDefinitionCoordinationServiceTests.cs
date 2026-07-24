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
    private readonly Mock<IFlowQueueOrchestrationService> flowQueueOrchestrationServiceMock;
    private readonly Mock<IFlowInstanceDataOrchestrationService> flowInstanceDataOrchestrationServiceMock;
    private readonly FlowDefinitionCoordinationService coordinationService;

    public FlowDefinitionCoordinationServiceTests()
    {
        flowQueueOrchestrationServiceMock =
            new Mock<IFlowQueueOrchestrationService>(behavior: MockBehavior.Strict);
        flowInstanceDataOrchestrationServiceMock =
            new Mock<IFlowInstanceDataOrchestrationService>(behavior: MockBehavior.Strict);
        coordinationService = new FlowDefinitionCoordinationService(
            flowQueueOrchestrationService: flowQueueOrchestrationServiceMock.Object,
            flowInstanceDataOrchestrationService: flowInstanceDataOrchestrationServiceMock.Object);
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>
            .CreateNew()
            .With(func: flow =>
                flow.Instances =
                [
                    Builder<FlowInstanceData>
                        .CreateNew()
                        .Build()
                ]
            )
            .Build();
}