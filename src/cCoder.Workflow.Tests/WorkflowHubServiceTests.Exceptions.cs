// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Models.Exceptions;
using cCoder.Workflow.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public sealed partial class WorkflowHubServiceTests
{
    public static TheoryData<Exception, Type> WorkflowHubExceptions =>
        new()
        {
            {
                new WorkflowValidationException(
                    innerException: new Exception()),
                typeof(WorkflowValidationException)
            },
            {
                new WorkflowDependencyException(
                    innerException: new Exception()),
                typeof(WorkflowDependencyException)
            },
            {
                new ValidationException(),
                typeof(WorkflowValidationException)
            },
            {
                new Exception(),
                typeof(WorkflowServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(WorkflowHubExceptions))]
    public async Task LogConnection_WhenDependencyFails_MapsException(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        // Given
        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                message: "New Client connected to WorkflowHub",
                args: It.IsAny<object[]>()))
            .Throws(exception: dependencyException);

        WorkflowHubService service = new(
            workflowHubBroker: workflowHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        // When
        Func<Task> action = async () =>
            await service.LogWorkflowHubConnectionAsync(
                isConnected: true);

        // Then
        Exception actualException = await Assert.ThrowsAnyAsync<Exception>(
            testCode: action);

        Assert.IsType(
            expectedType: expectedExceptionType,
            @object: actualException);
    }

    [Fact]
    public async Task SendMessage_WhenLevelIsMissing_ThrowsValidationException()
    {
        // Given
        Mock<IWorkflowHubBroker> workflowHubBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ILoggingBroker> loggingBrokerMock =
            new(behavior: MockBehavior.Strict);

        WorkflowHubService service = new(
            workflowHubBroker: workflowHubBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

        // When
        Func<Task> action = async () =>
            await service.SendWorkflowHubMessageAsync(
                level: null,
                message: "message",
                thread: "thread");

        // Then
        await Assert.ThrowsAsync<WorkflowValidationException>(
            testCode: action);
    }
}