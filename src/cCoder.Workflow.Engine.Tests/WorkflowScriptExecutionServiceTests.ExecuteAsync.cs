using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_DelegatesToWorkflowScriptExecutionOrchestrationService()
    {
        // Given
        const string payload = "return true";
        const string expectedResult = "true";
        orchestrationServiceMock
            .Setup(service => service.ExecuteAsync(payload, true))
            .ReturnsAsync(expectedResult);

        // When
        string actualResult = await workflowScriptExecutionService.ExecuteAsync(payload, useDetails: true);

        // Then
        actualResult.Should().Be(expectedResult);
        orchestrationServiceMock.Verify(service => service.ExecuteAsync(payload, true), Times.Once);
    }
}
