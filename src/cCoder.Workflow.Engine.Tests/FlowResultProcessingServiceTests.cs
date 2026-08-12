// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowResultProcessingServiceTests
{
    private readonly Mock<IWorkflowHttpClientBroker>
        workflowHttpClientBrokerMock =
            new(behavior: MockBehavior.Strict);

    private FlowResultProcessingService CreateService() =>
        new(workflowHttpClientBroker: workflowHttpClientBrokerMock.Object);

    private static FlowInstanceData CreateFlowInstanceData() =>
        new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            Name = "Flow",
            ContextString = "{}"
        };
}