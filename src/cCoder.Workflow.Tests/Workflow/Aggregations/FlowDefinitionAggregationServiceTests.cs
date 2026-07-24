// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.ServiceProviders;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;
using Moq;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

public partial class FlowDefinitionAggregationServiceTests
{
    private readonly Mock<IFlowDefinitionServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly Mock<IFlowDefinitionCoordinationService> flowDefinitionCoordinationServiceMock;
    private readonly Mock<IWorkflowMetadataTypeService> workflowMetadataTypeServiceMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FlowDefinitionAggregationService service;

    public FlowDefinitionAggregationServiceTests()
    {
        serviceProviderBrokerMock =
            new Mock<IFlowDefinitionServiceProviderBroker>(behavior: MockBehavior.Strict);
        flowDefinitionOrchestrationServiceMock =
            new Mock<IFlowDefinitionOrchestrationService>(behavior: MockBehavior.Strict);
        flowDefinitionCoordinationServiceMock =
            new Mock<IFlowDefinitionCoordinationService>(behavior: MockBehavior.Strict);
        workflowMetadataTypeServiceMock =
            new Mock<IWorkflowMetadataTypeService>(behavior: MockBehavior.Strict);
        authorizationBrokerMock =
            new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        service = new FlowDefinitionAggregationService(
            serviceProviderBroker: serviceProviderBrokerMock.Object);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetOperationService<IFlowDefinitionCoordinationService>(
                operation: FlowDefinitionOperation.Queue))
            .Returns(value: flowDefinitionCoordinationServiceMock.Object);
    }
}