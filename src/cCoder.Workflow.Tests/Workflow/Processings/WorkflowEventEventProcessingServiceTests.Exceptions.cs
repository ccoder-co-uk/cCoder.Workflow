// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventEventProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseWorkflowEventAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        WorkflowEvent entity = CreateRandomWorkflowEvent();

        workflowEventEventServiceMock
            .Setup(expression: dependency => dependency
                .RaiseWorkflowEventAddEventAsync(entity: entity))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseWorkflowEventAddEventAsync(entity: entity);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}