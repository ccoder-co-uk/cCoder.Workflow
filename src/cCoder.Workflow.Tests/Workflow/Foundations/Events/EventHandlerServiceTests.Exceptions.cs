// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models.Exceptions;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations.Events;

public sealed partial class EventHandlerServiceExceptionTests
{
    public static TheoryData<Exception, Type> ListenerDependencyExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), typeof(WorkflowValidationException) },
        { new WorkflowDependencyException(innerException: new Exception()), typeof(WorkflowDependencyException) },
        { new ValidationException(), typeof(WorkflowValidationException) },
        { new InvalidOperationException(), typeof(WorkflowDependencyException) },
        { new SecurityException(), typeof(SecurityException) },
        { new Exception(), typeof(WorkflowServiceException) }
    };

    [Theory]
    [MemberData(nameof(ListenerDependencyExceptions))]
    public void ListenToScheduledTaskExecuteEventsShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        var eventHubBrokerMock = new Mock<IEventHubBroker>();

        eventHubBrokerMock.Setup(expression: broker =>
                broker.ListenToEvent<ScheduledTask, IFlowDefinitionCoordinationService>(
                    "scheduled_task_execute",
                    It.IsAny<Func<IFlowDefinitionCoordinationService, ScheduledTask, ValueTask>>()))
            .Throws(exception: dependencyException);

        var service = new EventHandlerService(eventHubBroker: eventHubBrokerMock.Object);

        Action action = service.ListenToScheduledTaskExecuteEvents;

        Exception exception = action.Should().Throw<Exception>().Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005