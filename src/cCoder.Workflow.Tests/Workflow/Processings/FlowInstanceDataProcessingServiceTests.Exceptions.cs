// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowInstanceDataServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        // When
        Action action = () => flowInstanceDataProcessingService.GetAll();

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

        flowInstanceDataServiceMock
            .Setup(expression: service => service.AddQueuedFlowInstanceDataAsync(
                newFlowInstanceData: item))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowInstanceDataProcessingService
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
    public async Task ShouldMapDeleteAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        Guid id = Guid.NewGuid();

        flowInstanceDataServiceMock
            .Setup(expression: service => service.DeleteAsync(
                flowInstanceDataId: id))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowInstanceDataProcessingService
            .DeleteAsync(flowInstanceDataId: id);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}