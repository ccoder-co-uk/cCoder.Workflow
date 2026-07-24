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

public sealed partial class HealthTests
{
    [Fact]
    public async Task Run_ReturnsOk()
    {
        // Given
        TestHttpRequestData request = CreateRequest();

        TestHttpResponseData expectedResponse =
            (TestHttpResponseData)request.CreateResponse();

        expectedResponse.StatusCode = HttpStatusCode.OK;
        await expectedResponse.WriteStringAsync(value: "OK");

        processingServiceMock
            .Setup(expression: service =>
                service.ProcessHealthAsync(request: request))
            .ReturnsAsync(value: expectedResponse);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        ReadBody(response: response)
            .Should()
            .Be(expected: "OK");

        processingServiceMock.Verify(expression: service =>
            service.ProcessHealthAsync(request: request),
            times: Times.Once);
    }
}