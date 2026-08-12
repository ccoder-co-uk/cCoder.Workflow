// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowInstanceDataOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowInstanceDataProcessingServiceMock
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
    public async Task ShouldMapAddQueuedFlowInstanceDataAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowInstanceData item = CreateRandomFlowInstanceData();

        flowInstanceDataProcessingServiceMock
            .Setup(expression: service => service.AddQueuedFlowInstanceDataAsync(
                newEntity: item))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .AddQueuedFlowInstanceDataAsync(newEntity: item);

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
    public async Task ShouldMapDeleteAllFlowInstanceDataAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowInstanceData[] items = [CreateRandomFlowInstanceData()];

        flowInstanceDataProcessingServiceMock
            .Setup(expression: service => service.DeleteAllFlowInstanceDataAsync(
                deletedItems: items))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await orchestrationService
            .DeleteAllFlowInstanceDataAsync(deletedItems: items);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}