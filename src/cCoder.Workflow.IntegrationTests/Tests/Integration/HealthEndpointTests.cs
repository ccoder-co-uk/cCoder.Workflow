// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Integration;

[Collection(IntegrationAcceptanceCollection.Name)]
public sealed partial class HealthEndpointTests(IntegrationAcceptanceFixture fixture)
{
    [Fact]
    public async Task ShouldReturnOkFromAllApps()
    {
        // Given
        using HttpClient workflowClient = new()
        {
            BaseAddress = fixture.WorkflowBaseAddress
        };

        // When
        string web = await fixture.WebClient.GetStringAsync(requestUri: "Health");
        string hostedServices = await fixture.HostedServicesClient.GetStringAsync(requestUri: "Health");
        string workflow = await workflowClient.GetStringAsync(requestUri: "Health");

        // Then
        web.Should()
            .Be(expected: "OK");

        hostedServices.Should()
            .Be(expected: "OK");

        workflow.Should()
            .Be(expected: "OK");
    }
}