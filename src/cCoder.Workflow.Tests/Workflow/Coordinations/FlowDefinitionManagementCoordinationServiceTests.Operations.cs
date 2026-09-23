// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Coordinations;

public sealed partial class FlowDefinitionManagementCoordinationServiceTests
{
    [Fact]
    public void GetAllFlowDefinitions_WhenRequested_ReturnsDefinitions()
    {
        // Given
        IQueryable<FlowDefinition> expectedFlowDefinitions =
            new List<FlowDefinition>()
                .AsQueryable();

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: false))
            .Returns(value: expectedFlowDefinitions);

        // When
        IQueryable<FlowDefinition> actualFlowDefinitions =
            flowDefinitionManagementCoordinationService
                .GetAllFlowDefinitions();

        // Then
        Assert.Same(
            expected: expectedFlowDefinitions,
            actual: actualFlowDefinitions);

        flowDefinitionOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task UpdateFlowDefinition_WhenRequested_ReturnsDefinition()
    {
        // Given
        var flowDefinition = CreateFlowDefinition();

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.UpdateFlowDefinitionAsync(
                updatedFlowDefinition: flowDefinition))
            .ReturnsAsync(value: flowDefinition);

        // When
        FlowDefinition actualFlowDefinition =
            await flowDefinitionManagementCoordinationService
                .UpdateFlowDefinitionAsync(
                    updatedFlowDefinition: flowDefinition);

        // Then
        Assert.Same(
            expected: flowDefinition,
            actual: actualFlowDefinition);

        flowDefinitionOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ExecuteScript_WhenRequested_ReturnsResult()
    {
        // Given
        const string script = "return true;";
        const string expectedResult = "true";

        workflowInteractionOrchestrationServiceMock
            .Setup(expression: service => service.ExecuteScriptAsync(
                script: script))
            .ReturnsAsync(value: expectedResult);

        // When
        string actualResult =
            await flowDefinitionManagementCoordinationService
                .ExecuteScriptAsync(script: script);

        // Then
        Assert.Equal(
            expected: expectedResult,
            actual: actualResult);

        workflowInteractionOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ReadRequestBody_WhenRequested_ReturnsContent()
    {
        // Given
        await using Stream stream = new MemoryStream();
        const string expectedContent = "content";

        workflowInteractionOrchestrationServiceMock
            .Setup(expression: service => service.ReadRequestBodyAsync(
                stream: stream))
            .ReturnsAsync(value: expectedContent);

        // When
        string actualContent =
            await flowDefinitionManagementCoordinationService
                .ReadRequestBodyAsync(stream: stream);

        // Then
        Assert.Equal(
            expected: expectedContent,
            actual: actualContent);

        workflowInteractionOrchestrationServiceMock.VerifyAll();
    }
}