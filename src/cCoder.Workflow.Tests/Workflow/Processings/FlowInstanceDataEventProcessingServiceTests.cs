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

public partial class FlowInstanceDataEventProcessingServiceTests
{
    private readonly Mock<IFlowInstanceDataEventService> flowInstanceDataEventServiceMock;
    private readonly FlowInstanceDataEventProcessingService service;

    public FlowInstanceDataEventProcessingServiceTests()
    {
        flowInstanceDataEventServiceMock = new Mock<IFlowInstanceDataEventService>(MockBehavior.Strict);
        service = new FlowInstanceDataEventProcessingService(flowInstanceDataEventServiceMock.Object);
    }

    private static FlowInstanceData CreateRandomFlowInstanceData() =>
        Builder<FlowInstanceData>.CreateNew().Build();
}