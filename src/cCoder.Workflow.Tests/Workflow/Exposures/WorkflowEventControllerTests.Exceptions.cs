// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class WorkflowEventControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        workflowEventManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.Get(key: Guid.Empty);

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
        workflowEventManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync()
    {
        WorkflowEvent item = new();
        workflowEventManagerMock.Setup(expression: service => service.AddWorkflowEventAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync()
    {
        WorkflowEvent item = new();
        workflowEventManagerMock.Setup(expression: service => service.UpdateWorkflowEventAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(key: Guid.Empty, updatedEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync()
    {
        workflowEventManagerMock.Setup(expression: service => service.Get(workflowEventId: Guid.Empty))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(
            key: Guid.Empty,
            updatedDelta: new Delta<WorkflowEvent>());

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync()
    {
        workflowEventManagerMock.Setup(expression: service => service.DeleteAsync(workflowEventId: Guid.Empty))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005