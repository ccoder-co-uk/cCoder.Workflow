// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        actualHealth.Should()
            .Be(expected: "OK");
    }

    [Fact]
    public async Task GetHome_ReturnsHostedServiceReport()
    {
        // Given

        // When
        string actualHome = await GetHomeAsync();

        // Then
        actualHome.Should()
            .Contain(expected: "Workflow Hosted Services");

        actualHome.Should()
            .Contain(expected: "InstanceMaintenanceManagement");

        actualHome.Should()
            .Contain(expected: "QueueInstanceManagement");

        actualHome.Should()
            .Contain(expected: "ScheduledTaskRunnerManagement");

        actualHome.Should()
            .Contain(expected: "flow_instance_data_add");
    }
}