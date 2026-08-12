// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class CalendarEventControllerTests
{
    [Fact]
    public void ShouldReturnMetadataWhenGetMetadataIsRequested()
    {
        IActionResult result = controller.GetMetadata();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnExtendedMetadataWhenGetMetadataIsExtended()
    {
        controller.Request.QueryString = new QueryString(value: "?extend=true");

        IActionResult result = controller.GetMetadata();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnCalendarEventWhenGetFindsRequestedCalendarEvent()
    {
        CalendarEvent calendarEvent = new() { Id = 1 };
        calendarEventManagerMock.Setup(expression: service => service.GetAll(false))
            .Returns(value: new[] { calendarEvent }.AsQueryable());

        IActionResult result = controller.Get(key: calendarEvent.Id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedCalendarEvent()
    {
        calendarEventManagerMock.Setup(expression: service => service.GetAll(false))
            .Returns(value: Array.Empty<CalendarEvent>().AsQueryable());

        IActionResult result = controller.Get(key: 1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newEntity: new CalendarEvent());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: 1, updatedEntity: new CalendarEvent());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchCannotFindCalendarEventAsync()
    {
        calendarEventManagerMock.Setup(expression: service => service.Get(calendarEventId: 1))
            .Returns(value: null);

        IActionResult result = await controller.Put(key: 1, updatedDelta: new Delta<CalendarEvent>());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldUpdateCalendarEventEventWhenPatchFindsCalendarAsync()
    {
        CalendarEvent calendarEvent = new() { Id = 1 };
        calendarEventManagerMock.Setup(expression: service => service.Get(calendarEventId: 1))
            .Returns(value: calendarEvent);
        calendarEventManagerMock.Setup(expression: service => service.UpdateCalendarEventAsync(calendarEvent))
            .ReturnsAsync(value: calendarEvent);

        IActionResult result = await controller.Put(key: 1, updatedDelta: new Delta<CalendarEvent>());

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        calendarEventManagerMock.Setup(expression: service => service.DeleteAsync(calendarEventId: 1))
            .Returns(value: ValueTask.CompletedTask);

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeOfType<NoContentResult>();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005