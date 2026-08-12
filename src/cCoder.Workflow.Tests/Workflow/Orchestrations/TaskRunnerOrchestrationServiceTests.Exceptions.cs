// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Services.Orchestrations;

public partial class TaskRunnerOrchestrationServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        cCoder.Core.Services.Tests.Workflow.Foundations
            .FlowDefinitionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapRunFailureAsync(
        Exception exception,
        Type expectedType)
    {
        // Given
        scheduledTaskProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await taskRunnerOrchestrationService
            .RunAsync();

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}