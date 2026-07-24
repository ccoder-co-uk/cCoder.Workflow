// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Workflow.AcceptanceTests.Infrastructure;
using Xunit;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteScriptTests
{
    [Fact]
    public async Task Run_ExecutesScriptService()
    {
        // Given
        const string payload = "return true";
        const string expected = "true";
        TestHttpRequestData request = CreateRequest(payload: payload);

        TestHttpResponseData expectedResponse =
            (TestHttpResponseData)request.CreateResponse();

        expectedResponse.StatusCode = HttpStatusCode.OK;
        await expectedResponse.WriteStringAsync(value: expected);

        processingServiceMock
            .Setup(expression: service =>
                service.ProcessExecuteScriptAsync(
                    request: request,
                    useDetails: true))
            .ReturnsAsync(value: expectedResponse);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request: request, useDetails: true);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        response.ReadBody()
            .Should()
            .Be(expected: expected);

        processingServiceMock.Verify(expression: service =>
            service.ProcessExecuteScriptAsync(
                request: request,
                useDetails: true),
            times: Times.Once);
    }
}