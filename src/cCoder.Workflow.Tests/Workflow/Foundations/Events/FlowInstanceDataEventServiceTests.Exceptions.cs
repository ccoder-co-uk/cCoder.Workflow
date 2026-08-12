// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowInstanceDataEventServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseFlowInstanceDataAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowInstanceData entity = new();

        flowInstanceDataEventBrokerMock
            .Setup(expression: broker => broker.RaiseFlowInstanceDataAddEventAsync(
                message: It.Is<EventMessage<FlowInstanceData>>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseFlowInstanceDataAddEventAsync(entity: entity);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}