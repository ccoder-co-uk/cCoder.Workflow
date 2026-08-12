// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionEventProcessingServiceTests
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
        FlowDefinition flowDefinition = CreateRandomFlowDefinition();

        flowDefinitionEventServiceMock
            .Setup(expression: foundation => foundation.RaiseFlowDefinitionAddEventAsync(
                entity: flowDefinition))
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