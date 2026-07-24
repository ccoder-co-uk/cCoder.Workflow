// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Coordinations;

public partial class WorkflowEventCoordinationServiceTests
{
    private readonly Mock<IWorkflowEventOrchestrationService> workflowEventOrchestrationServiceMock;
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly WorkflowEventCoordinationService coordinationService;

    public WorkflowEventCoordinationServiceTests()
    {
        workflowEventOrchestrationServiceMock =
            new Mock<IWorkflowEventOrchestrationService>(MockBehavior.Strict);

        flowDefinitionOrchestrationServiceMock =
            new Mock<IFlowDefinitionOrchestrationService>(MockBehavior.Strict);

        coordinationService = new WorkflowEventCoordinationService(
            workflowEventOrchestrationServiceMock.Object,
            flowDefinitionOrchestrationServiceMock.Object);
    }

    private static Page CreateRandomPage() =>
        Builder<Page>.CreateNew()
            .With(func: x => x.Id = 1)
            .With(func: x => x.AppId = 1)
            .With(func: x => x.Name = "Home")
            .With(func: x => x.Path = "home")
            .Build();

    private static WorkflowEvent CreateSubscription(Page page, string executeAs = "system") =>
        CreateSubscription(page: page, flowId: Guid.NewGuid(), executeAs: executeAs);

    private static WorkflowEvent CreateSubscription(Page page, Guid flowId, string executeAs = "system") =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = "Any",
            EventContext = $"page_update{page.Path}",
            FlowId = flowId,
            ExecuteAs = executeAs,
            ExecuteAsUser = new User { Id = executeAs },
        };
}
