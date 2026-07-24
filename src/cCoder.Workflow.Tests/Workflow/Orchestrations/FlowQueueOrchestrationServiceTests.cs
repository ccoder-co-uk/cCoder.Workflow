// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    private readonly FlowQueueOrchestrationService orchestrationService;

    public FlowQueueOrchestrationServiceTests()
    {
        flowDefinitionProcessingServiceMock =
            new Mock<IFlowDefinitionProcessingService>(MockBehavior.Strict);

        flowInstanceDataProcessingServiceMock =
            new Mock<IFlowInstanceDataProcessingService>(MockBehavior.Strict);

        orchestrationService = new FlowQueueOrchestrationService(
            flowDefinitionProcessingServiceMock.Object,
            flowInstanceDataProcessingServiceMock.Object);
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