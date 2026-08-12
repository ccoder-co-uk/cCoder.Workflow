// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class CalendarEventControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        calendarEventManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.Get(key: 1);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ShouldReturnServerErrorWhenGetMetadataFails()
    {
        controller.ControllerContext = new ControllerContext();

        IActionResult result = controller.GetMetadata();

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ShouldReturnServerErrorWhenGetAllFails()
    {
        calendarEventManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync()
    {
        CalendarEvent item = new();
        calendarEventManagerMock.Setup(expression: service => service.AddCalendarEventAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync()
    {
        CalendarEvent item = new();
        calendarEventManagerMock.Setup(expression: service => service.UpdateCalendarEventAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(key: 1, updatedEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync()
    {
        calendarEventManagerMock.Setup(expression: service => service.Get(calendarEventId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(
            key: 1,
            updatedDelta: new Delta<CalendarEvent>());

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync()
    {
        calendarEventManagerMock.Setup(expression: service => service.DeleteAsync(calendarEventId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005