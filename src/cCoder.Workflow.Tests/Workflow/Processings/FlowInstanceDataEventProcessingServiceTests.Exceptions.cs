// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataEventProcessingServiceTests
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
        FlowInstanceData entity = CreateRandomFlowInstanceData();

        flowInstanceDataEventServiceMock
            .Setup(expression: dependency => dependency
                .RaiseFlowInstanceDataAddEventAsync(entity: entity))
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