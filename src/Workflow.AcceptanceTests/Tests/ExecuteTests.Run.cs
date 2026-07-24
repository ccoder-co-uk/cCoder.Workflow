// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Workflow.AcceptanceTests.Infrastructure;
using Xunit;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteTests
{
    [Fact]
    public async Task Run_ExecutesFlowRunner()
    {
        // Given
        WorkflowRequest requestPayload = new()
        {
            InstanceId = Guid.NewGuid(),
            Api = "https://localhost/",
            AuthToken = "token",
        };

        TestHttpRequestData request = CreateRequest(request: requestPayload);

        TestHttpResponseData expectedResponse =
            (TestHttpResponseData)request.CreateResponse();

        expectedResponse.StatusCode = HttpStatusCode.OK;
        await expectedResponse.WriteStringAsync(value: "OK");

        processingServiceMock
            .Setup(expression: service =>
                service.ProcessExecuteAsync(request: request))
            .ReturnsAsync(value: expectedResponse);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        response.ReadBody()
            .Should()
            .Be(expected: "OK");

        processingServiceMock.Verify(expression: service =>
            service.ProcessExecuteAsync(request: request),
            times: Times.Once);
    }
}