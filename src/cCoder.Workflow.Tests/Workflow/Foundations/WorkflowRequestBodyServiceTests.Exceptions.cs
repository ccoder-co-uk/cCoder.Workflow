// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowRequestBodyServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        WorkflowScriptExecutionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ReadTextAsync_WhenBrokerFails_MapsException(
        Exception exception,
        Type expectedType)
    {
        // Given
        using var stream = new MemoryStream();

        var brokerMock = new Mock<IStreamBroker>(
            behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.ReadTextAsync(
                stream: stream))
            .Throws(exception: exception);

        var service = new WorkflowRequestBodyService(
            streamBroker: brokerMock.Object);

        // When
        Func<Task> action = async () => await service.ReadTextAsync(
            stream: stream);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}