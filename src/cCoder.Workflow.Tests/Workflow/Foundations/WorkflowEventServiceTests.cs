// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Brokers;
using AuthorizationBroker = cCoder.Workflow.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    private readonly Mock<IWorkflowEventBroker> workflowEventBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly WorkflowEventService workflowEventService;

    public WorkflowEventServiceTests()
    {
        workflowEventBrokerMock = new Mock<IWorkflowEventBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);
        workflowEventService = new WorkflowEventService(
            workflowEventBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static WorkflowEvent CreateRandomWorkflowEvent(Guid workflowEventId = default, Guid flowId = default)
    {
        WorkflowEvent workflowEvent = Builder<WorkflowEvent>
            .CreateNew()
            .With(func: x => x.Id = workflowEventId == Guid.Empty ? Guid.NewGuid() : workflowEventId)
            .With(func: x => x.FlowId = flowId == Guid.Empty ? Guid.NewGuid() : flowId)
            .With(func: x => x.Type = $"Type-{Guid.NewGuid():N}")
            .With(func: x => x.EventContext = $"Context-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedBy = $"creator-{Guid.NewGuid():N}")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.ExecuteAs = $"user-{Guid.NewGuid():N}")
            .Build();

        return workflowEvent;
    }
}