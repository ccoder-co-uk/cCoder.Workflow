// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Models.Exceptions;
using FluentAssertions;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Exposures;

public sealed partial class ControllerFailureLoggingTests
{
    public static TheoryData<Exception, int> FailureExceptions => new()
    {
        { new WorkflowValidationException(innerException: new Exception()), StatusCodes.Status400BadRequest },
        { new SecurityException(), StatusCodes.Status403Forbidden },
        { new Exception(), StatusCodes.Status500InternalServerError }
    };

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void CalendarControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new CalendarController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void CalendarControllerShouldLogGetFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new CalendarController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.Get(key: 1);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void CalendarEventControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarEventManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new CalendarEventController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void CalendarEventControllerShouldLogGetFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarEventManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new CalendarEventController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.Get(key: 1);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void FlowDefinitionControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IFlowDefinitionManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAllFlowDefinitions())
            .Throws(exception: exception);

        var controller = new FlowDefinitionController(
            service: serviceMock.Object,
            authInfo: Mock.Of<ISSOAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void FlowInstanceDataControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IFlowInstanceDataManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new FlowInstanceDataController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void ScheduledTaskControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IScheduledTaskManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new ScheduledTaskController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void WorkflowEventControllerShouldLogGetAllFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IWorkflowEventManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: false))
            .Throws(exception: exception);

        var controller = new WorkflowEventController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetAll(queryOptions: null);

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void MetadataControllerShouldLogKnownActivityTypeFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IWorkflowMetadataTypeManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetKnownActivityTypes())
            .Throws(exception: exception);

        var controller = new FlowDefinitionMetadataController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetKnownActivityTypes();

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public void MetadataControllerShouldLogKnownSystemTypeFailures(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IWorkflowMetadataTypeManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        serviceMock.Setup(expression: service => service.GetKnownSystemTypes())
            .Throws(exception: exception);

        var controller = new FlowDefinitionMetadataController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult result = controller.GetKnownSystemTypes();

        // Then
        VerifyFailure(result: result, exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task CalendarControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var calendar = new Calendar();

        serviceMock.Setup(expression: service => service.AddCalendarAsync(newEntity: calendar))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateCalendarAsync(updatedEntity: calendar))
            .ThrowsAsync(exception: exception);

        var controller = new CalendarController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: calendar);
        IActionResult putResult = await controller.Put(key: 1, updatedEntity: calendar);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task CalendarEventControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<ICalendarEventManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var calendarEvent = new CalendarEvent();

        serviceMock.Setup(expression: service => service.AddCalendarEventAsync(newEntity: calendarEvent))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateCalendarEventAsync(updatedEntity: calendarEvent))
            .ThrowsAsync(exception: exception);

        var controller = new CalendarEventController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: calendarEvent);
        IActionResult putResult = await controller.Put(key: 1, updatedEntity: calendarEvent);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task FlowDefinitionControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IFlowDefinitionManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var flowDefinition = new FlowDefinition();

        serviceMock.Setup(expression: service => service.AddFlowDefinitionAsync(newEntity: flowDefinition))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateFlowDefinitionAsync(updatedEntity: flowDefinition))
            .ThrowsAsync(exception: exception);

        var controller = new FlowDefinitionController(
            service: serviceMock.Object,
            authInfo: Mock.Of<ISSOAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: flowDefinition);
        IActionResult putResult = await controller.Put(key: Guid.NewGuid(), updatedEntity: flowDefinition);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task FlowInstanceDataControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IFlowInstanceDataManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var flowInstanceData = new FlowInstanceData();

        serviceMock.Setup(expression: service => service.AddFlowInstanceDataAsync(newEntity: flowInstanceData))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateFlowInstanceDataAsync(updatedEntity: flowInstanceData))
            .ThrowsAsync(exception: exception);

        var controller = new FlowInstanceDataController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: flowInstanceData);
        IActionResult putResult = await controller.Put(key: Guid.NewGuid(), updatedEntity: flowInstanceData);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ScheduledTaskControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IScheduledTaskManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var scheduledTask = new ScheduledTask();

        serviceMock.Setup(expression: service => service.AddScheduledTaskAsync(newEntity: scheduledTask))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateScheduledTaskAsync(updatedEntity: scheduledTask))
            .ThrowsAsync(exception: exception);

        var controller = new ScheduledTaskController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: scheduledTask);
        IActionResult putResult = await controller.Put(key: 1, updatedEntity: scheduledTask);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task WorkflowEventControllerShouldLogWriteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var serviceMock = new Mock<IWorkflowEventManager>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var workflowEvent = new WorkflowEvent();

        serviceMock.Setup(expression: service => service.AddWorkflowEventAsync(newEntity: workflowEvent))
            .ThrowsAsync(exception: exception);

        serviceMock.Setup(expression: service => service.UpdateWorkflowEventAsync(updatedEntity: workflowEvent))
            .ThrowsAsync(exception: exception);

        var controller = new WorkflowEventController(service: serviceMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult postResult = await controller.Post(newEntity: workflowEvent);
        IActionResult putResult = await controller.Put(key: Guid.NewGuid(), updatedEntity: workflowEvent);

        // Then
        VerifyFailures(results: [postResult, putResult], exception: exception, expectedStatusCode: expectedStatusCode, loggingBrokerMock: loggingBrokerMock);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ControllersShouldLogDeleteFailuresAsync(Exception exception, int expectedStatusCode)
    {
        // Given
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var calendarManagerMock = new Mock<ICalendarManager>();
        var calendarEventManagerMock = new Mock<ICalendarEventManager>();
        var flowDefinitionManagerMock = new Mock<IFlowDefinitionManager>();
        var flowInstanceDataManagerMock = new Mock<IFlowInstanceDataManager>();
        var scheduledTaskManagerMock = new Mock<IScheduledTaskManager>();
        var workflowEventManagerMock = new Mock<IWorkflowEventManager>();
        Guid key = Guid.NewGuid();

        calendarManagerMock.Setup(expression: service => service.Get(calendarId: 1))
            .Throws(exception: exception);

        calendarEventManagerMock.Setup(expression: service => service.Get(calendarEventId: 1))
            .Throws(exception: exception);

        flowDefinitionManagerMock.Setup(expression: service => service.GetFlowDefinition(flowDefinitionId: key))
            .Throws(exception: exception);

        flowInstanceDataManagerMock.Setup(expression: service => service.Get(flowInstanceDataId: key))
            .Throws(exception: exception);

        scheduledTaskManagerMock.Setup(expression: service => service.Get(scheduledTaskId: 1))
            .Throws(exception: exception);

        workflowEventManagerMock.Setup(expression: service => service.Get(workflowEventId: key))
            .Throws(exception: exception);

        calendarManagerMock.Setup(expression: service => service.DeleteAsync(calendarId: 1))
            .ThrowsAsync(exception: exception);

        calendarEventManagerMock.Setup(expression: service => service.DeleteAsync(calendarEventId: 1))
            .ThrowsAsync(exception: exception);

        flowDefinitionManagerMock.Setup(expression: service => service.DeleteFlowDefinitionAsync(flowDefinitionId: key))
            .ThrowsAsync(exception: exception);

        flowDefinitionManagerMock.Setup(expression: service => service.QueueFlowDefinitionAsync(
            flowDefinitionId: key,
            asUserId: null,
            args: string.Empty))
            .ThrowsAsync(exception: exception);

        flowInstanceDataManagerMock.Setup(expression: service => service.DeleteAsync(flowInstanceDataId: key))
            .ThrowsAsync(exception: exception);

        scheduledTaskManagerMock.Setup(expression: service => service.DeleteAsync(scheduledTaskId: 1))
            .ThrowsAsync(exception: exception);

        scheduledTaskManagerMock.Setup(expression: service => service.ExecuteAsync(
            scheduledTaskId: 1,
            incrementNextExecution: true))
            .ThrowsAsync(exception: exception);

        workflowEventManagerMock.Setup(expression: service => service.DeleteAsync(workflowEventId: key))
            .ThrowsAsync(exception: exception);

        var calendarController = new CalendarController(service: calendarManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var calendarEventController = new CalendarEventController(service: calendarEventManagerMock.Object, loggingBroker: loggingBrokerMock.Object);

        var flowDefinitionController = new FlowDefinitionController(
            service: flowDefinitionManagerMock.Object,
            authInfo: Mock.Of<ISSOAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        flowDefinitionController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var flowInstanceDataController = new FlowInstanceDataController(service: flowInstanceDataManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var scheduledTaskController = new ScheduledTaskController(service: scheduledTaskManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var workflowEventController = new WorkflowEventController(service: workflowEventManagerMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult calendarResult = await calendarController.Delete(key: 1);
        IActionResult calendarEventResult = await calendarEventController.Delete(key: 1);
        IActionResult flowDefinitionResult = await flowDefinitionController.Delete(key: key);
        IActionResult flowInstanceDataResult = await flowInstanceDataController.Delete(key: key);
        IActionResult scheduledTaskResult = await scheduledTaskController.Delete(key: 1);
        IActionResult workflowEventResult = await workflowEventController.Delete(key: key);
        IActionResult calendarDeltaResult = await calendarController.Put(key: 1, updatedDelta: (Delta<Calendar>)null);
        IActionResult calendarEventDeltaResult = await calendarEventController.Put(key: 1, updatedDelta: (Delta<CalendarEvent>)null);
        IActionResult flowDefinitionDeltaResult = await flowDefinitionController.Put(key: key, updatedDelta: (Delta<FlowDefinition>)null);
        IActionResult flowInstanceDataDeltaResult = await flowInstanceDataController.Put(key: key, updatedDelta: (Delta<FlowInstanceData>)null);
        IActionResult scheduledTaskDeltaResult = await scheduledTaskController.Put(key: 1, updatedDelta: (Delta<ScheduledTask>)null);
        IActionResult workflowEventDeltaResult = await workflowEventController.Put(key: key, updatedDelta: (Delta<WorkflowEvent>)null);
        IActionResult scheduledTaskExecutionResult = await scheduledTaskController.PostAsync(key: 1);
        IActionResult flowDefinitionExecutionResult = await flowDefinitionController.PostAsync(key: key);

        // Then
        VerifyFailures(
            results:
            [
                calendarResult,
                calendarEventResult,
                flowDefinitionResult,
                flowInstanceDataResult,
                scheduledTaskResult,
                workflowEventResult,
                calendarDeltaResult,
                calendarEventDeltaResult,
                flowDefinitionDeltaResult,
                flowInstanceDataDeltaResult,
                scheduledTaskDeltaResult,
                workflowEventDeltaResult,
                scheduledTaskExecutionResult,
                flowDefinitionExecutionResult
            ],
            exception: exception,
            expectedStatusCode: expectedStatusCode,
            loggingBrokerMock: loggingBrokerMock);
    }

    [Fact]
    public async Task ControllersShouldLogAdditionalUnhandledFailuresAsync()
    {
        // Given
        var exception = new Exception();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        var calendarManagerMock = new Mock<ICalendarManager>();
        var calendarEventManagerMock = new Mock<ICalendarEventManager>();
        var flowDefinitionManagerMock = new Mock<IFlowDefinitionManager>();
        var flowInstanceDataManagerMock = new Mock<IFlowInstanceDataManager>();
        var scheduledTaskManagerMock = new Mock<IScheduledTaskManager>();
        var workflowEventManagerMock = new Mock<IWorkflowEventManager>();
        Guid key = Guid.NewGuid();

        calendarManagerMock.Setup(expression: service => service.Get(calendarId: 1))
            .Throws(exception: exception);

        calendarEventManagerMock.Setup(expression: service => service.Get(calendarEventId: 1))
            .Throws(exception: exception);

        flowDefinitionManagerMock.Setup(expression: service => service.GetFlowDefinition(flowDefinitionId: key))
            .Throws(exception: exception);

        flowInstanceDataManagerMock.Setup(expression: service => service.Get(flowInstanceDataId: key))
            .Throws(exception: exception);

        scheduledTaskManagerMock.Setup(expression: service => service.Get(scheduledTaskId: 1))
            .Throws(exception: exception);

        workflowEventManagerMock.Setup(expression: service => service.Get(workflowEventId: key))
            .Throws(exception: exception);

        scheduledTaskManagerMock.Setup(expression: service => service.ExecuteAsync(
            scheduledTaskId: 1,
            incrementNextExecution: true))
            .ThrowsAsync(exception: exception);

        var calendarController = new CalendarController(service: calendarManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var calendarEventController = new CalendarEventController(service: calendarEventManagerMock.Object, loggingBroker: loggingBrokerMock.Object);

        var flowDefinitionController = new FlowDefinitionController(
            service: flowDefinitionManagerMock.Object,
            authInfo: Mock.Of<ISSOAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object);

        var flowInstanceDataController = new FlowInstanceDataController(service: flowInstanceDataManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var scheduledTaskController = new ScheduledTaskController(service: scheduledTaskManagerMock.Object, loggingBroker: loggingBrokerMock.Object);
        var workflowEventController = new WorkflowEventController(service: workflowEventManagerMock.Object, loggingBroker: loggingBrokerMock.Object);

        // When
        IActionResult calendarResult = await calendarController.Put(key: 1, updatedDelta: (Delta<Calendar>)null);
        IActionResult calendarEventResult = await calendarEventController.Put(key: 1, updatedDelta: (Delta<CalendarEvent>)null);
        IActionResult flowDefinitionResult = await flowDefinitionController.Put(key: key, updatedDelta: (Delta<FlowDefinition>)null);
        IActionResult flowInstanceDataResult = await flowInstanceDataController.Put(key: key, updatedDelta: (Delta<FlowInstanceData>)null);
        IActionResult scheduledTaskResult = await scheduledTaskController.Put(key: 1, updatedDelta: (Delta<ScheduledTask>)null);
        IActionResult workflowEventResult = await workflowEventController.Put(key: key, updatedDelta: (Delta<WorkflowEvent>)null);
        IActionResult flowExecutionResult = await flowDefinitionController.PostAsync(key: Guid.NewGuid());
        IActionResult scheduledExecutionResult = await scheduledTaskController.PostAsync(key: 1);

        // Then
        VerifyUnhandledFailures(
            results:
            [
                calendarResult,
                calendarEventResult,
                flowDefinitionResult,
                flowInstanceDataResult,
                scheduledTaskResult,
                workflowEventResult,
                flowExecutionResult,
                scheduledExecutionResult
            ],
            loggingBrokerMock: loggingBrokerMock);
    }

    private static void VerifyUnhandledFailures(
        IActionResult[] results,
        Mock<ILoggingBroker> loggingBrokerMock)
    {
        foreach (IActionResult result in results)
        {
            result
                .Should()
                .BeAssignableTo<IStatusCodeActionResult>()
                .Which.StatusCode
                .Should()
                .Be(expected: StatusCodes.Status500InternalServerError);
        }

        loggingBrokerMock.Verify(expression: broker => broker.LogError(
            exception: It.IsAny<Exception>(),
            message: "Controller request failed.",
            args: It.IsAny<object[]>()), times: Times.Exactly(callCount: results.Length));
    }

    private static void VerifyFailures(
        IActionResult[] results,
        Exception exception,
        int expectedStatusCode,
        Mock<ILoggingBroker> loggingBrokerMock)
    {
        foreach (IActionResult result in results)
        {
            result
                .Should()
                .BeAssignableTo<IStatusCodeActionResult>()
                .Which.StatusCode
                .Should()
                .Be(expected: expectedStatusCode);
        }

        loggingBrokerMock.Verify(expression: broker => broker.LogError(
            exception: exception,
            message: "Controller request failed.",
            args: It.IsAny<object[]>()), times: Times.Exactly(callCount: results.Length));
    }

    private static void VerifyFailure(
        IActionResult result,
        Exception exception,
        int expectedStatusCode,
        Mock<ILoggingBroker> loggingBrokerMock)
    {
        result
            .Should()
            .BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode
            .Should()
            .Be(expected: expectedStatusCode);

        loggingBrokerMock.Verify(expression: broker => broker.LogError(
            exception: exception,
            message: "Controller request failed.",
            args: It.IsAny<object[]>()), times: Times.Once);
    }
}