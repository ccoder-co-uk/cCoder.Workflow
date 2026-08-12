// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class WorkflowEventEventServiceTests
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
        WorkflowEvent entity = new();

        workflowEventEventBrokerMock
            .Setup(expression: broker => broker.RaiseWorkflowEventAddEventAsync(
                message: It.Is<EventMessage<WorkflowEvent>>(match: _ => true)))
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