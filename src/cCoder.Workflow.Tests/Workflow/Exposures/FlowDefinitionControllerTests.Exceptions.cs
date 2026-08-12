// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class FlowDefinitionControllerTests
{
    [Fact]
    public void ShouldReturnServerErrorWhenGetFails()
    {
        flowDefinitionManagerMock.Setup(expression: service =>
                service.GetFlowDefinition(flowDefinitionId: Guid.Empty))
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
        flowDefinitionManagerMock.Setup(expression: service => service.GetAllFlowDefinitions())
            .Throws(exception: new Exception());

        IActionResult result = controller.GetAll(queryOptions: null);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPostFailsAsync(Exception exception, int expectedStatusCode)
    {
        FlowDefinition item = new();
        flowDefinitionManagerMock.Setup(expression: service => service.AddFlowDefinitionAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Post(newEntity: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPutFailsAsync(Exception exception, int expectedStatusCode)
    {
        FlowDefinition item = new();
        flowDefinitionManagerMock.Setup(expression: service => service.UpdateFlowDefinitionAsync(item))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(key: Guid.Empty, updatedEntity: item);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenPatchFailsAsync(Exception exception, int expectedStatusCode)
    {
        flowDefinitionManagerMock.Setup(expression: service => service.GetFlowDefinition(flowDefinitionId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Put(
            key: Guid.Empty,
            updatedDelta: new Delta<FlowDefinition>());

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenDeleteFailsAsync(Exception exception, int expectedStatusCode)
    {
        flowDefinitionManagerMock.Setup(expression: service => service.DeleteFlowDefinitionAsync(flowDefinitionId: Guid.Empty))
            .Throws(exception: exception);

        IActionResult result = await controller.Delete(key: Guid.Empty);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenExecuteFailsAsync(
        Exception exception,
        int expectedStatusCode)
    {
        flowDefinitionManagerMock.Setup(expression: service => service.QueueFlowDefinitionAsync(
                flowDefinitionId: Guid.Empty,
                asUserId: It.IsAny<string>(),
                args: It.IsAny<string>()))
            .Throws(exception: exception);

        IActionResult result = await controller.PostAsync(key: Guid.Empty);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(FailureExceptions))]
    public async Task ShouldReturnServerErrorWhenExecuteScriptFailsAsync(
        Exception exception,
        int expectedStatusCode)
    {
        flowDefinitionManagerMock.Setup(expression: service =>
                service.ExecuteScriptAsync(It.IsAny<string>()))
            .Throws(exception: exception);

        IActionResult result = await controller.PostScript();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005