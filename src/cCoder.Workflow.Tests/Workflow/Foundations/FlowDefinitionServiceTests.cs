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
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.AppId = appId)
            .With(x => x.DefinitionJson = "{}")
            .With(x => x.ConfigJson = "{}")
            .With(x => x.ReportingComponentName = $"report-{Guid.NewGuid():N}")
            .With(x => x.InstanceReportingComponentName = $"instance-report-{Guid.NewGuid():N}")
            .With(x => x.Name = $"FlowDefinition-{Guid.NewGuid():N}")
            .With(x => x.CreatedBy = "tester")
            .With(x => x.LastUpdatedBy = "tester")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5))
            .With(x => x.LastUpdated = DateTimeOffset.UtcNow)
            .Build();

        return flowDefinition;
    }
}













