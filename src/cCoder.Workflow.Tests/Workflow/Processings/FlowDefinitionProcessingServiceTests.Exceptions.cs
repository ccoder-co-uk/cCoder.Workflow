// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowDefinitionServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        // When
        Action action = () => flowDefinitionProcessingService.GetAll();

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

        flowDefinitionServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: flowDefinition))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowDefinitionProcessingService
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
        flowDefinitionServiceMock
            .Setup(expression: service => service
                .DeleteWithInstancesByAppIdAsync(appId: 7))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowDefinitionProcessingService
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