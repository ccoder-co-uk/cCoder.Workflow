// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Planning;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync(Exception exception, int expectedStatusCode)
    {
        ScheduledTask item = new();
        scheduledTaskManagerMock.Setup(expression: service => service.AddScheduledTaskAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync(Exception exception, int expectedStatusCode)
    {
        ScheduledTask item = new();
        scheduledTaskManagerMock.Setup(expression: service => service.UpdateScheduledTaskAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(key: 1, updatedEntity: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync(Exception exception, int expectedStatusCode)
    {
        scheduledTaskManagerMock.Setup(expression: service => service.Get(scheduledTaskId: 1))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(
            key: 1,
            updatedDelta: new Delta<ScheduledTask>());

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync(Exception exception, int expectedStatusCode)
    {
        scheduledTaskManagerMock.Setup(expression: service => service.DeleteAsync(scheduledTaskId: 1))
            .Throws(exception: exception);

        IActionResult result = await controller.Delete(key: 1);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
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