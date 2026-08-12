// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class WorkflowEventOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        workflowEventProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        // When
        Action action = () => orchestrationService.GetAll();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddWorkflowEventAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        WorkflowEvent item = CreateRandomWorkflowEvent();

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.AddWorkflowEventAsync(
                newEntity: item))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .AddWorkflowEventAsync(newEntity: item);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteAllWorkflowEventAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        WorkflowEvent[] items = [CreateRandomWorkflowEvent()];

        workflowEventProcessingServiceMock
            .Setup(expression: service => service.DeleteAllWorkflowEventAsync(
                deletedItems: items))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .DeleteAllWorkflowEventAsync(deletedItems: items);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}