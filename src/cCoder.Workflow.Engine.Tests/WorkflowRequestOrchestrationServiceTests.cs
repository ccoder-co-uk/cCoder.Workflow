// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestOrchestrationServiceTests
{
    private readonly Mock<IFlowCommunicationProcessingService>
        flowCommunicationProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

    private readonly Mock<IFlowInstanceProcessingService>
        flowInstanceProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

    private readonly Mock<IFlowResultProcessingService>
        flowResultProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

    private WorkflowRequestOrchestrationService CreateService() =>
        new WorkflowRequestOrchestrationService(
            flowCommunicationProcessingService:
                flowCommunicationProcessingServiceMock.Object,
            flowInstanceProcessingService:
                flowInstanceProcessingServiceMock.Object,
            flowResultProcessingService:
                flowResultProcessingServiceMock.Object);

    private static WorkflowRequest CreateWorkflowRequest() =>
        new(
            api: "https://localhost/",
            token: "token",
            flowId: Guid.NewGuid(),
            instanceId: Guid.NewGuid());

    private static FlowExecution CompleteExecution(FlowExecution execution)
    {
        execution.Result = new cCoder.Data.Models.Workflow.FlowInstanceData()
        {
            Id = execution.Request.InstanceId,
            FlowDefinitionId = execution.Request.FlowId
        };

        return execution;
    }
}