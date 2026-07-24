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

public partial class FlowInstanceDataOrchestrationServiceTests
{
    private readonly Mock<IFlowInstanceDataProcessingService> flowInstanceDataProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataEventProcessingService> flowInstanceDataEventProcessingServiceMock;
    private readonly FlowInstanceDataOrchestrationService orchestrationService;

    public FlowInstanceDataOrchestrationServiceTests()
    {
        flowInstanceDataProcessingServiceMock = new Mock<IFlowInstanceDataProcessingService>(MockBehavior.Strict);
        flowInstanceDataEventProcessingServiceMock = new Mock<IFlowInstanceDataEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new FlowInstanceDataOrchestrationService(
            flowInstanceDataProcessingServiceMock.Object,
            flowInstanceDataEventProcessingServiceMock.Object
        );
    }

    private static FlowInstanceData CreateRandomFlowInstanceData() =>
        Builder<FlowInstanceData>.CreateNew()
            .Build();
}