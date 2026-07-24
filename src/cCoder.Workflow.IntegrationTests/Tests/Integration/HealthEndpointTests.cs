// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Integration;

[Collection(IntegrationAcceptanceCollection.Name)]
public sealed class HealthEndpointTests(IntegrationAcceptanceFixture fixture)
{
    [Fact]
    public async Task ShouldReturnOkFromAllApps()
    {
        string web = await fixture.WebClient.GetStringAsync(requestUri: "Health");
        string hostedServices = await fixture.HostedServicesClient.GetStringAsync(requestUri: "Health");

        using HttpClient workflowClient = new() { BaseAddress = fixture.WorkflowBaseAddress };
        string workflow = await workflowClient.GetStringAsync(requestUri: "Health");

        web.Should()
            .Be(expected: "OK");
        hostedServices.Should()
            .Be(expected: "OK");
        workflow.Should()
            .Be(expected: "OK");
    }
}