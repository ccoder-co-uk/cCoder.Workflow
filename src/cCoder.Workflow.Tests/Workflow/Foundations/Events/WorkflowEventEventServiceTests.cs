// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class WorkflowEventEventServiceTests
{
    private readonly Mock<IWorkflowEventEventBroker> workflowEventEventBrokerMock;
    private readonly cCoder.Workflow.Services.Foundations.Events.WorkflowEventEventService service;
    private const string CurrentUserId = "test-user";

    public WorkflowEventEventServiceTests()
    {
        workflowEventEventBrokerMock = new Mock<IWorkflowEventEventBroker>(behavior: MockBehavior.Strict);

        workflowEventEventBrokerMock
            .Setup(expression: broker => broker.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        service = new cCoder.Workflow.Services.Foundations.Events.WorkflowEventEventService(
            workflowEventEventBroker: workflowEventEventBrokerMock.Object);
    }
}