// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using Moq;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

public partial class FlowDefinitionAggregationServiceTests
{
    private readonly Mock<IFlowDefinitionCoordinationService> flowDefinitionCoordinationServiceMock;
    private readonly Mock<IFlowDefinitionManagementCoordinationService> flowDefinitionManagementCoordinationServiceMock;
    private readonly FlowDefinitionAggregationService service;

    public FlowDefinitionAggregationServiceTests()
    {
        flowDefinitionCoordinationServiceMock =
            new Mock<IFlowDefinitionCoordinationService>(behavior: MockBehavior.Strict);
        flowDefinitionManagementCoordinationServiceMock =
            new Mock<IFlowDefinitionManagementCoordinationService>(behavior: MockBehavior.Strict);

        service = new FlowDefinitionAggregationService(
            queueCoordinationService: flowDefinitionCoordinationServiceMock.Object,
            managementCoordinationService: flowDefinitionManagementCoordinationServiceMock.Object);
    }
}