// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models.Exceptions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToServiceForDeleteAsync()
    {
        Guid flowId = Guid.NewGuid();

        flowDefinitionServiceMock
            .Setup(expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: flowId))
            .Returns(value: ValueTask.CompletedTask);

        await flowDefinitionProcessingService.DeleteAsync(flowDefinitionId: flowId);

        flowDefinitionServiceMock.Verify(
expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: flowId),
times: Times.Once
        );

        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldWrapDependencyExceptionFromServiceForDeleteAsync()
    {
        // Given
        Guid flowId = Guid.NewGuid();
        InvalidOperationException dependencyException = new("boom");

        flowDefinitionServiceMock
            .Setup(expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: flowId))
            .Returns(value: ValueTask.FromException(exception: dependencyException));

        // When
        WorkflowDependencyException actualException =
            await Assert.ThrowsAsync<WorkflowDependencyException>(
                testCode: async () =>
                    await flowDefinitionProcessingService.DeleteAsync(
                        flowDefinitionId: flowId));

        // Then
        Assert.Same(
            expected: dependencyException,
            actual: actualException.InnerException);

        flowDefinitionServiceMock.Verify(
            expression: x => x.DeleteWithInstancesAsync(flowDefinitionId: flowId),
            times: Times.Once);

        flowDefinitionServiceMock.VerifyNoOtherCalls();
    }
}