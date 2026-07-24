// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
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

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        ReadBody(response: response)
            .Should()
            .Be(expected: "OK");
    }
}