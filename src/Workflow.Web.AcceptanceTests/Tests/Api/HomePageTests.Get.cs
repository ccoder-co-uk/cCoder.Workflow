using FluentAssertions;
using System.Net;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class HomePageTests
{
    [Fact]
    public async Task Get_RedirectsToWorkflowTester()
    {
        // Given

        // When
        using HttpResponseMessage response = await GetHomeAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.Should().Be("/tools/index.html");
    }

    [Fact]
    public async Task GetTools_ReturnsWorkflowTester()
    {
        // Given

        // When
        string actualHome = await GetToolsAsync();

        // Then
        actualHome.Should().Contain("<title>Workflow</title>");
        actualHome.Should().Contain("/tools/api.js");
        actualHome.Should().Contain("/tools/grids.js");
        actualHome.Should().Contain("Flow definition and execution management");
    }
}
