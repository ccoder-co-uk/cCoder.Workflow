// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowInstanceDataControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFlowInstanceDataCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFlowInstanceData()
    {
        // Given

        // When
        IReadOnlyList<FlowInstanceData> actualInstances = await GetFlowInstanceDataAsync(top: 1);

        // Then
        actualInstances.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFlowInstanceDataById()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(includeInstance: true);

        // When
        FlowInstanceData actualInstance = await GetFlowInstanceDataAsync(flowInstanceDataId: seededContext.InstanceId);

        // Then
        actualInstance.Should().NotBeNull();
        actualInstance.Id.Should().Be(expected: seededContext.InstanceId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(
            includeInstance: true,
            "flowinstancedata_create",
            "flowinstancedata_update",
            "flowinstancedata_delete");

        int actualStatusCode = await GetFlowInstanceDataStatusCodeAsync(flowInstanceDataId: seededContext.InstanceId);

        actualStatusCode.Should().Be(expected: (int)HttpStatusCode.NotFound);

        await Teardown(seededContext: seededContext);
    }
}