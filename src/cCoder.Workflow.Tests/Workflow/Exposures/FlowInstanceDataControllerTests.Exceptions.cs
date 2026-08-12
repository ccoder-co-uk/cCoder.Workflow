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

public partial class FlowInstanceDataControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        flowInstanceDataManagerMock.Setup(expression: service => service.GetAll())
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
        flowInstanceDataManagerMock.Setup(expression: service => service.GetAll())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync()
    {
        FlowInstanceData item = new();
        flowInstanceDataManagerMock.Setup(expression: service => service.AddFlowInstanceDataAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync()
    {
        FlowInstanceData item = new();
        flowInstanceDataManagerMock.Setup(expression: service => service.UpdateFlowInstanceDataAsync(item))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(key: Guid.Empty, updatedEntity: item);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync()
    {
        flowInstanceDataManagerMock.Setup(expression: service => service.Get(flowInstanceDataId: Guid.Empty))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Put(
            key: Guid.Empty,
            updatedDelta: new Delta<FlowInstanceData>());

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync()
    {
        flowInstanceDataManagerMock.Setup(expression: service => service.DeleteAsync(flowInstanceDataId: Guid.Empty))
            .Throws(exception: new Exception());

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005