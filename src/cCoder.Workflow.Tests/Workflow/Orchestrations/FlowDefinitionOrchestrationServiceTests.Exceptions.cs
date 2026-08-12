// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowDefinitionProcessingServiceMock
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
    public async Task ShouldMapAddFlowDefinitionAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition();

        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newEntity: flowDefinition))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .AddFlowDefinitionAsync(newEntity: flowDefinition);

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
    public async Task ShouldMapDeleteByAppIdAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        flowDefinitionProcessingServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: 7))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .DeleteByAppIdAsync(appId: 7);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}