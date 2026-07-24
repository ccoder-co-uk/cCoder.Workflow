// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
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
        scriptExecutionServiceMock
            .Setup(service => service.ExecuteAsync(payload, true))
            .ReturnsAsync(expected);

        TestHttpRequestData request = CreateRequest(payload);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request, useDetails: true);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReadBody().Should().Be(expected);
        scriptExecutionServiceMock.Verify(service => service.ExecuteAsync(payload, true), Times.Once);
    }
}