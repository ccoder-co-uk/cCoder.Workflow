// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowRunnerTests
{
    private readonly Mock<IWorkflowRequestOrchestrationService> workflowRequestOrchestrationServiceMock = new();
    private readonly FlowRunner flowRunner;

    public FlowRunnerTests() =>
        flowRunner = new FlowRunner(workflowRequestOrchestrationServiceMock.Object);

    private static WorkflowRequest CreateWorkflowRequest() =>
        new()
        {
            InstanceId = Guid.NewGuid(),
            Api = "https://localhost/",
            AuthToken = "token",
        };
}