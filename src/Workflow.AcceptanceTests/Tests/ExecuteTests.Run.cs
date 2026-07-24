// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
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
        TestHttpRequestData request = CreateRequest(requestPayload);

        // When
        TestHttpResponseData response = (TestHttpResponseData)await function.Run(request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReadBody().Should().Be("OK");
        flowRunnerMock.Verify(runner =>
            runner.RunAsync(It.Is<WorkflowRequest>(actual =>
                actual.InstanceId == requestPayload.InstanceId
                && actual.Api == requestPayload.Api
                && actual.AuthToken == requestPayload.AuthToken)),
            Times.Once);
    }
}