using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using JsonBroker = cCoder.Workflow.Brokers.JsonBroker;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    private readonly Mock<IFlowDefinitionProcessingService> flowDefinitionProcessingServiceMock;
    private readonly Mock<IFlowDefinitionEventProcessingService> flowDefinitionEventProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataProcessingService> flowInstanceDataProcessingServiceMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FlowDefinitionOrchestrationService orchestrationService;

    public FlowDefinitionOrchestrationServiceTests()
    {
        flowDefinitionProcessingServiceMock = new Mock<IFlowDefinitionProcessingService>(MockBehavior.Strict);
        flowDefinitionEventProcessingServiceMock = new Mock<IFlowDefinitionEventProcessingService>(MockBehavior.Strict);
        flowInstanceDataProcessingServiceMock = new Mock<IFlowInstanceDataProcessingService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        orchestrationService = new FlowDefinitionOrchestrationService(
            flowDefinitionProcessingServiceMock.Object,
            flowDefinitionEventProcessingServiceMock.Object,
            flowInstanceDataProcessingServiceMock.Object,
            authorizationBrokerMock.Object,
            new JsonBroker()
        );
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>.CreateNew().Build();
}












