// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models.Exceptions;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowScriptExecutionServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            { new WorkflowValidationException(new Exception()), typeof(WorkflowValidationException) },
            { new WorkflowDependencyException(new Exception()), typeof(WorkflowDependencyException) },
            { new ValidationException(), typeof(WorkflowValidationException) },
            { new InvalidOperationException(), typeof(WorkflowDependencyException) },
            { new SecurityException(), typeof(SecurityException) },
            { new Exception(), typeof(WorkflowServiceException) }
        };

    [Fact]
    public async Task ExecuteAsync_WhenBrokerSucceeds_ReturnsResult()
    {
        // Given
        var brokerMock = new Mock<IWorkflowHttpClientBroker>(
            behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.PostTextAsync(
                apiRoot: "https://localhost/",
                timeout: TimeSpan.FromMinutes(minutes: 10),
                requestUri: "ExecuteScript",
                content: "script"))
            .Returns(value: ValueTask.FromResult(result: "result"));

        var service = new WorkflowScriptExecutionService(
            httpClientBroker: brokerMock.Object);

        // When
        string result = await service.ExecuteAsync(
            serviceUrl: "https://localhost/",
            script: "script");

        // Then
        result
            .Should()
            .Be(expected: "result");
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ExecuteAsync_WhenBrokerFails_MapsException(
        Exception exception,
        Type expectedType)
    {
        // Given
        var brokerMock = new Mock<IWorkflowHttpClientBroker>(
            behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.PostTextAsync(
                apiRoot: "https://localhost/",
                timeout: TimeSpan.FromMinutes(minutes: 10),
                requestUri: "ExecuteScript",
                content: "script"))
            .Throws(exception: exception);

        var service = new WorkflowScriptExecutionService(
            httpClientBroker: brokerMock.Object);

        // When
        Func<Task> action = async () => await service.ExecuteAsync(
            serviceUrl: "https://localhost/",
            script: "script");

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}