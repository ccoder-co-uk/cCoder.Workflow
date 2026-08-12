// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public partial class FlowDefinitionEventServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRaiseFlowDefinitionAddEventAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowDefinition flowDefinition = new() { Id = Guid.NewGuid() };

        flowDefinitionEventBrokerMock
            .Setup(expression: broker => broker.RaiseFlowDefinitionAddEventAsync(
                message: It.Is<EventMessage<FlowDefinition>>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .RaiseFlowDefinitionAddEventAsync(entity: flowDefinition);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}