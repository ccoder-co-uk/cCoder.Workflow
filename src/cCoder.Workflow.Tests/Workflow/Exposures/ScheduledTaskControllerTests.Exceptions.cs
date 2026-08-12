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

public partial class ScheduledTaskControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        scheduledTaskManagerMock.Setup(expression: service => service.GetAll())
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
        scheduledTaskManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync()
    {
        ScheduledTask item = new();
        scheduledTaskManagerMock.Setup(expression: service => service.AddScheduledTaskAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync()
    {
        ScheduledTask item = new();
        scheduledTaskManagerMock.Setup(expression: service => service.UpdateScheduledTaskAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(key: 1, updatedEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync()
    {
        scheduledTaskManagerMock.Setup(expression: service => service.Get(scheduledTaskId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(
            key: 1,
            updatedDelta: new Delta<ScheduledTask>());

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync()
    {
        scheduledTaskManagerMock.Setup(expression: service => service.DeleteAsync(scheduledTaskId: 1))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenExecuteFailsAsync()
    {
        scheduledTaskManagerMock.Setup(expression: service =>
                service.ExecuteAsync(scheduledTaskId: 1, incrementNextExecution: true))
            .Throws(exception: new Exception());

        IActionResult result = await controller.PostAsync(
            key: 1,
            incrementNextExecution: true);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005