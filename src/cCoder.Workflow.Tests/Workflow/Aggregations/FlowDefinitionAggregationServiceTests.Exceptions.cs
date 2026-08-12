// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Services.Orchestrations;
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
        serviceProviderBrokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: FlowDefinitionOperation.Crud))
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
        serviceProviderBrokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: FlowDefinitionOperation.Crud))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await service
            .AddFlowDefinitionAsync(newEntity: new FlowDefinition());

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
        serviceProviderBrokerMock
            .Setup(expression: broker => broker
                .GetOperationService<IFlowDefinitionOrchestrationService>(
                    operation: FlowDefinitionOperation.Crud))
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