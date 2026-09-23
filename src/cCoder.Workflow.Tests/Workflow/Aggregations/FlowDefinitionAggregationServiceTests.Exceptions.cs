// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

#pragma warning disable STXFORMAT009
public partial class FlowDefinitionAggregationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetFlowDefinitionFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.GetFlowDefinition(
                flowDefinitionId: It.IsAny<Guid>()))
            .Throws(exception: exception);

        // When
        Action action = () => service.GetFlowDefinition(
            flowDefinitionId: Guid.NewGuid());

        // Then
        action.Should().Throw<Exception>().Which
            .Should().BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddFlowDefinitionFailureAsync(
        Exception exception,
        Type expectedType)
    {
        // Given
        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: It.IsAny<FlowDefinition>()))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .AddFlowDefinitionAsync(newFlowDefinition: new FlowDefinition());

        // Then
        Exception thrown = (await action.Should().ThrowAsync<Exception>()).Which;
        thrown.Should().BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteFlowDefinitionFailureAsync(
        Exception exception,
        Type expectedType)
    {
        // Given
        flowDefinitionManagementCoordinationServiceMock
            .Setup(expression: service => service.DeleteFlowDefinitionAsync(
                flowDefinitionId: It.IsAny<Guid>()))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .DeleteFlowDefinitionAsync(flowDefinitionId: Guid.NewGuid());

        // Then
        Exception thrown = (await action.Should().ThrowAsync<Exception>()).Which;
        thrown.Should().BeOfType(expectedType: expectedType);
    }
}
#pragma warning restore STXFORMAT009