// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using AuthorizationBroker = cCoder.Workflow.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowDefinitionServiceTests
{
    private readonly Mock<IFlowDefinitionBroker> flowDefinitionBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FlowDefinitionService flowDefinitionService;

    public FlowDefinitionServiceTests()
    {
        flowDefinitionBrokerMock = new Mock<IFlowDefinitionBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        flowDefinitionService = new FlowDefinitionService(
            flowDefinitionBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static FlowDefinition CreateRandomFlowDefinition(Guid id = default, int appId = 7)
    {
        FlowDefinition flowDefinition = Builder<FlowDefinition>
            .CreateNew()
            .With(func:x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(func:x => x.AppId = appId)
            .With(func:x => x.DefinitionJson = "{}")
            .With(func:x => x.ConfigJson = "{}")
            .With(func:x => x.ReportingComponentName = $"report-{Guid.NewGuid():N}")
            .With(func:x => x.InstanceReportingComponentName = $"instance-report-{Guid.NewGuid():N}")
            .With(func:x => x.Name = $"FlowDefinition-{Guid.NewGuid():N}")
            .With(func:x => x.CreatedBy = "tester")
            .With(func:x => x.LastUpdatedBy = "tester")
            .With(func:x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5))
            .With(func:x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return flowDefinition;
    }
}