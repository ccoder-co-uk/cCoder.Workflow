// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowQueueOrchestrationServiceTests
{
    private readonly Mock<IFlowDefinitionProcessingService> flowDefinitionProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataProcessingService> flowInstanceDataProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataEventProcessingService>
        flowInstanceDataEventProcessingServiceMock;
    private readonly FlowQueueOrchestrationService orchestrationService;

    public FlowQueueOrchestrationServiceTests()
    {
        flowDefinitionProcessingServiceMock =
            new Mock<IFlowDefinitionProcessingService>(behavior: MockBehavior.Strict);

        flowInstanceDataProcessingServiceMock =
            new Mock<IFlowInstanceDataProcessingService>(behavior: MockBehavior.Strict);

        flowInstanceDataEventProcessingServiceMock =
            new Mock<IFlowInstanceDataEventProcessingService>(
                behavior: MockBehavior.Strict);

        orchestrationService = new FlowQueueOrchestrationService(
            flowDefinitionProcessingService: flowDefinitionProcessingServiceMock.Object,
            flowInstanceDataProcessingService: flowInstanceDataProcessingServiceMock.Object,
            flowInstanceDataEventProcessingService:
                flowInstanceDataEventProcessingServiceMock.Object);
    }

    private static FlowDefinition CreateFlowDefinition(Guid flowDefinitionId) =>
        new()
        {
            Id = flowDefinitionId,
            AppId = 1,
            DefinitionJson = "{}"
        };

    private static Flow CreateFlow() =>
        new()
        {
            Activities = [new Start()]
        };
}

#pragma warning restore STXFORMAT005, STXFORMAT009, STXTEST005