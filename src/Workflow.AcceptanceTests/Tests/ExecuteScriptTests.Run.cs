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
            .Setup(expression:service => service.ExecuteAsync(payload, true))
            .ReturnsAsync(value:expected);

        TestHttpRequestData request = CreateRequest(payload:payload);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request:request, useDetails: true);

        // Then
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK);
        response.ReadBody().Should().Be(expected:expected);
        scriptExecutionServiceMock.Verify(expression:service => service.ExecuteAsync(payload, true), times:Times.Once);
    }
}