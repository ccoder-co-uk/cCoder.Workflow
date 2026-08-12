// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowInstanceDataBrokerMock
            .Setup(expression: broker => broker.SelectAllFlowInstanceData())
            .Throws(exception: exception);

        // When
        Action action = () => flowInstanceDataService.GetAll();

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
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        flowInstanceDataBrokerMock
            .Setup(expression: broker => broker.AddFlowInstanceDataAsync(
                newEntity: It.Is<FlowInstanceData>(match: _ => true)))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowInstanceDataService
            .AddQueuedFlowInstanceDataAsync(
                newFlowInstanceData: flowInstanceData);

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
    public async Task ShouldMapDeleteAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        flowInstanceDataBrokerMock
            .Setup(expression: broker => broker.SelectAllFlowInstanceData())
            .Returns(value: new[] { flowInstanceData }.AsQueryable());

        flowInstanceDataBrokerMock
            .Setup(expression: broker => broker.SelectAppId(
                entity: flowInstanceData))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowInstanceDataService
            .DeleteAsync(flowInstanceDataId: flowInstanceData.Id);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}