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

public partial class CalendarControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        calendarManagerMock.Setup(expression: service => service.GetAll())
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
        calendarManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync()
    {
        Calendar item = new();
        calendarManagerMock.Setup(expression: service => service.AddCalendarAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync()
    {
        Calendar item = new();
        calendarManagerMock.Setup(expression: service => service.UpdateCalendarAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(key: 1, updatedEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync()
    {
        calendarManagerMock.Setup(expression: service => service.Get(calendarId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(
            key: 1,
            updatedDelta: new Delta<Calendar>());

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync()
    {
        calendarManagerMock.Setup(expression: service => service.DeleteAsync(calendarId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005