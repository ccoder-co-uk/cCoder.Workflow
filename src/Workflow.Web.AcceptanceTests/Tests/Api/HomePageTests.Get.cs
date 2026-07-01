using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class HomePageTests
{
    [Fact]
    public async Task Get_ReturnsWorkflowTester()
    {
        // Given

        // When
        string actualHome = await GetHomeAsync();

        // Then
        actualHome.Should().Contain("<title>Workflow</title>");
        actualHome.Should().Contain("flowManagement.js");
    }
}
