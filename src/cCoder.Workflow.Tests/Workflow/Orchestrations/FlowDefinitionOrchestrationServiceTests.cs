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

public partial class FlowDefinitionOrchestrationServiceTests
{
    private readonly Mock<IFlowDefinitionProcessingService> flowDefinitionProcessingServiceMock;
    private readonly Mock<IFlowDefinitionEventProcessingService> flowDefinitionEventProcessingServiceMock;
    private readonly FlowDefinitionOrchestrationService orchestrationService;

    public FlowDefinitionOrchestrationServiceTests()
    {
        flowDefinitionProcessingServiceMock = new Mock<IFlowDefinitionProcessingService>(MockBehavior.Strict);
        flowDefinitionEventProcessingServiceMock = new Mock<IFlowDefinitionEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new FlowDefinitionOrchestrationService(
            flowDefinitionProcessingServiceMock.Object,
            flowDefinitionEventProcessingServiceMock.Object);
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>.CreateNew()
            .Build();
}