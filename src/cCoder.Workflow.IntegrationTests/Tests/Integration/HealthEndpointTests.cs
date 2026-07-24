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
        string web = await fixture.WebClient.GetStringAsync("Health");
        string hostedServices = await fixture.HostedServicesClient.GetStringAsync("Health");

        using HttpClient workflowClient = new() { BaseAddress = fixture.WorkflowBaseAddress };
        string workflow = await workflowClient.GetStringAsync("Health");

        web.Should().Be("OK");
        hostedServices.Should().Be("OK");
        workflow.Should().Be("OK");
    }
}