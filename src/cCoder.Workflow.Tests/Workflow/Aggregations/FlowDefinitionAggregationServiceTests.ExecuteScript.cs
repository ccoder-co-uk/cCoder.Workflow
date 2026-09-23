// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

public partial class FlowDefinitionAggregationServiceTests
{
    [Fact]
    public async Task ShouldExecuteScriptThroughWorkflowApiAsync()
    {
        // Given
        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.ExecuteScriptAsync(
                script: "return 1;"))
            .ReturnsAsync(value: "ok");

        // When
        string result = await service.ExecuteScriptAsync(script: "return 1;");

        // Then
        result.Should()
            .Be(expected: "ok");

        flowDefinitionManagementCoordinationServiceMock.VerifyAll();
    }
}