// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class WorkflowEventServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        workflowEventBrokerMock
            .Setup(expression: broker => broker.SelectAllWorkflowEvents())
            .Throws(exception: exception);

        // When
        Action action = () => workflowEventService.GetAll();

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
    public async Task ShouldMapAddWorkflowEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression: broker => broker.SelectAppId(
                entity: workflowEvent))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await workflowEventService
            .AddWorkflowEventAsync(newWorkflowEvent: workflowEvent);

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
    public async Task ShouldMapDeleteAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock
            .Setup(expression: broker => broker.SelectAllWorkflowEvents())
            .Returns(value: new[] { workflowEvent }.AsQueryable());

        workflowEventBrokerMock
            .Setup(expression: broker => broker.SelectAppId(
                entity: workflowEvent))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await workflowEventService.DeleteAsync(
            workflowEventId: workflowEvent.Id);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}