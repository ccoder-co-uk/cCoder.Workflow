// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Coordinations;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowRequestOrchestrationServiceTests
{
    private readonly Mock<IFlowExecutionOrchestrationService>
        flowInstanceProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

    private readonly Mock<IWorkflowLifecycleOrchestrationService>
        workflowLifecycleOrchestrationServiceMock =
            new(behavior: MockBehavior.Strict);

    private WorkflowRequestCoordinationService CreateService() =>
        new(
            flowExecutionOrchestrationService:
                flowInstanceProcessingServiceMock.Object,
            workflowLifecycleOrchestrationService:
                workflowLifecycleOrchestrationServiceMock.Object);

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

#pragma warning restore STXFORMAT005