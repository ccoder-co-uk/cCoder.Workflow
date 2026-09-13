// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Services.Foundations;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowResultServiceTests
{
    private readonly Mock<IWorkflowHttpClientBroker>
        workflowHttpClientBrokerMock =
            new(behavior: MockBehavior.Strict);

    private FlowResultService CreateService() =>
        new(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: new JsonBroker());

    private static FlowInstanceData CreateFlowInstanceData() =>
        new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            Name = "Flow",
            ContextString = "{}"
        };
}