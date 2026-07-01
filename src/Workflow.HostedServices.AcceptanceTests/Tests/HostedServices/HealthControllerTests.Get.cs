using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.HostedServices;

public sealed partial class HealthControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Given

        // When
        string actualHealth = await GetHealthAsync();

        // Then
        actualHealth.Should().Be("OK");
    }

    [Fact]
    public async Task GetHome_ReturnsHostedServiceReport()
    {
        // Given

        // When
        string actualHome = await GetHomeAsync();

        // Then
        actualHome.Should().Contain("Workflow Hosted Services");
        actualHome.Should().Contain("InstanceMaintenanceManagement");
        actualHome.Should().Contain("QueueInstanceManagement");
        actualHome.Should().Contain("flow_instance_data_add");
    }
}
