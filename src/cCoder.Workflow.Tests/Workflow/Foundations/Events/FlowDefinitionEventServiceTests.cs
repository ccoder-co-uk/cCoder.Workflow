// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using cCoder.Data;
using Moq;
using cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowDefinitionEventServiceTests
{
    private readonly Mock<IFlowDefinitionEventBroker> flowDefinitionEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.FlowDefinitionEventService service;
    private const string CurrentUserId = "test-user";

    public FlowDefinitionEventServiceTests()
    {
        flowDefinitionEventBrokerMock = new Mock<IFlowDefinitionEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        flowDefinitionEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(expression:x => x.SSOUserId).Returns(value:CurrentUserId);
        service = new cCoder.Workflow.Services.Foundations.Events.FlowDefinitionEventService(
            flowDefinitionEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}