// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowResultProcessingServiceTests
{
    [Fact]
    public async Task ShouldSaveFlowInstanceDataAsync()
    {
        // Given
        const string apiRoot = "https://localhost/";
        const string authToken = "token";
        FlowInstanceData instanceData = CreateFlowInstanceData();
        string capturedPayload = null;

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.PutJsonAsync(
                apiRoot: apiRoot,
                authToken: authToken,
                requestUri: $"Workflow/FlowInstanceData({instanceData.Id})",
                payload: It.IsAny<string>()))
            .Callback<string, string, string, string>(
                action: (_, _, _, payload) => capturedPayload = payload)
            .Returns(value: ValueTask.FromResult(
                result: new WorkflowHttpResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Status = "OK"
                }));

        var service = CreateService();

        // When
        await service.SaveFlowInstanceDataAsync(
            flowInstanceData: instanceData,
            apiRoot: apiRoot,
            authToken: authToken);

        // Then
        capturedPayload
            .Should()
            .Contain(expected: instanceData.Id.ToString());

        capturedPayload
            .Should()
            .Contain(expected: instanceData.FlowDefinitionId.ToString());

        workflowHttpClientBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRejectUnsuccessfulFlowInstanceDataSaveAsync()
    {
        // Given
        const string apiRoot = "https://localhost/";
        const string authToken = "token";
        FlowInstanceData instanceData = CreateFlowInstanceData();

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.PutJsonAsync(
                apiRoot: apiRoot,
                authToken: authToken,
                requestUri: It.IsAny<string>(),
                payload: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(
                result: new WorkflowHttpResult
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Status = "InternalServerError",
                    Body = "failed"
                }));

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .SaveFlowInstanceDataAsync(
                flowInstanceData: instanceData,
                apiRoot: apiRoot,
                authToken: authToken);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();
    }

    [Theory]
    [InlineData(null, "https://localhost/", "token")]
    [InlineData("instance", "", "token")]
    [InlineData("instance", "https://localhost/", " ")]
    public async Task ShouldRejectInvalidFlowInstanceDataSaveAsync(
        string instanceMarker,
        string apiRoot,
        string authToken)
    {
        // Given
        FlowInstanceData instanceData = instanceMarker is null
            ? null
            : CreateFlowInstanceData();

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .SaveFlowInstanceDataAsync(
                flowInstanceData: instanceData,
                apiRoot: apiRoot,
                authToken: authToken);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();

        workflowHttpClientBrokerMock.VerifyNoOtherCalls();
    }
}