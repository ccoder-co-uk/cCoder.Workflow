// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowCommunicationProcessingServiceTests
{
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    private readonly Mock<IWorkflowHubConnectionBroker>
        workflowHubConnectionBrokerMock =
            new(behavior: MockBehavior.Strict);

    private FlowCommunicationProcessingService CreateService() =>
        new(
            logger: loggingBrokerMock.Object,
            workflowHubConnectionBroker:
                workflowHubConnectionBrokerMock.Object);

    private static WorkflowRequest CreateWorkflowRequest() =>
        new(
            api: "https://localhost/",
            token: "token",
            flowId: Guid.NewGuid(),
            instanceId: Guid.NewGuid());
}