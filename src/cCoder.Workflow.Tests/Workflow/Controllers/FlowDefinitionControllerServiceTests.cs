using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Tests.Workflow.Controllers;

public partial class FlowDefinitionControllerServiceTests
{
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly Mock<IFlowDefinitionCoordinationService> flowDefinitionCoordinationServiceMock;
    private readonly Mock<IWorkflowMetadataTypeService> workflowMetadataTypeServiceMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FlowDefinitionControllerService service;

    public FlowDefinitionControllerServiceTests()
    {
        flowDefinitionOrchestrationServiceMock =
            new Mock<IFlowDefinitionOrchestrationService>(MockBehavior.Strict);
        flowDefinitionCoordinationServiceMock =
            new Mock<IFlowDefinitionCoordinationService>(MockBehavior.Strict);
        workflowMetadataTypeServiceMock =
            new Mock<IWorkflowMetadataTypeService>(MockBehavior.Strict);
        authorizationBrokerMock =
            new Mock<IAuthorizationBroker>(MockBehavior.Strict);

        service = new FlowDefinitionControllerService(
            flowDefinitionOrchestrationServiceMock.Object,
            flowDefinitionCoordinationServiceMock.Object,
            workflowMetadataTypeServiceMock.Object,
            authorizationBrokerMock.Object,
            new Config { Services = new Dictionary<string, string>() });
    }
}
