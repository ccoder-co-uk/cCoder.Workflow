// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using AuthorizationBroker = cCoder.Workflow.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    private readonly Mock<IFlowInstanceDataBroker> flowInstanceDataBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FlowInstanceDataService flowInstanceDataService;

    public FlowInstanceDataServiceTests()
    {
        flowInstanceDataBrokerMock = new Mock<IFlowInstanceDataBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        flowInstanceDataService = new FlowInstanceDataService(
            flowInstanceDataBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static FlowInstanceData CreateRandomFlowInstanceData(
        Guid id = default,
        Guid flowDefinitionId = default
    )
    {
        FlowInstanceData flowInstanceData = Builder<FlowInstanceData>
            .CreateNew()
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.FlowDefinitionId = flowDefinitionId == Guid.Empty ? Guid.NewGuid() : flowDefinitionId)
            .With(x => x.Name = $"FlowInstance-{Guid.NewGuid():N}")
            .With(x => x.State = "Queued")
            .With(x => x.ReportingComponentName = $"report-{Guid.NewGuid():N}")
            .With(x => x.Caller = "tester")
            .With(x => x.ContextString = "{}")
            .With(x => x.Start = DateTimeOffset.UtcNow)
            .With(x => x.End = default(DateTimeOffset))
            .Build();

        return flowInstanceData;
    }
}