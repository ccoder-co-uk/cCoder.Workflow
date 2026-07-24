// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionEventProcessingServiceTests
{
    private readonly Mock<IFlowDefinitionEventService> flowDefinitionEventServiceMock;
    private readonly FlowDefinitionEventProcessingService service;

    public FlowDefinitionEventProcessingServiceTests()
    {
        flowDefinitionEventServiceMock = new Mock<IFlowDefinitionEventService>(behavior: MockBehavior.Strict);
        service = new FlowDefinitionEventProcessingService(flowDefinitionEventServiceMock.Object);
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>.CreateNew()
            .Build();
}