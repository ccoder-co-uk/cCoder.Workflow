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

public partial class CalendarControllerTests
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
    public void ShouldReturnCalendarWhenGetFindsRequestedCalendar()
    {
        Calendar calendar = new() { Id = 1 };
        calendarManagerMock.Setup(expression: service => service.GetAll(false))
            .Returns(value: new[] { calendar }.AsQueryable());

        IActionResult result = controller.Get(key: calendar.Id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ShouldReturnNotFoundWhenGetCannotFindRequestedCalendar()
    {
        calendarManagerMock.Setup(expression: service => service.GetAll(false))
            .Returns(value: Array.Empty<Calendar>().AsQueryable());

        IActionResult result = controller.Get(key: 1);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPostModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Post(newEntity: new Calendar());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPutModelIsInvalidAsync()
    {
        controller.ModelState.AddModelError(key: "Name", errorMessage: "Required");

        IActionResult result = await controller.Put(key: 1, updatedEntity: new Calendar());

        result.Should().BeAssignableTo<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchCannotFindCalendarAsync()
    {
        calendarManagerMock.Setup(expression: service => service.Get(calendarId: 1))
            .Returns(value: null);

        IActionResult result = await controller.Put(key: 1, updatedDelta: new Delta<Calendar>());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldUpdateCalendarWhenPatchFindsCalendarAsync()
    {
        Calendar calendar = new() { Id = 1 };
        calendarManagerMock.Setup(expression: service => service.Get(calendarId: 1))
            .Returns(value: calendar);
        calendarManagerMock.Setup(expression: service => service.UpdateCalendarAsync(calendar))
            .ReturnsAsync(value: calendar);

        IActionResult result = await controller.Put(key: 1, updatedDelta: new Delta<Calendar>());

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeleteSucceedsAsync()
    {
        calendarManagerMock.Setup(expression: service => service.DeleteAsync(calendarId: 1))
            .Returns(value: ValueTask.CompletedTask);

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeOfType<NoContentResult>();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005