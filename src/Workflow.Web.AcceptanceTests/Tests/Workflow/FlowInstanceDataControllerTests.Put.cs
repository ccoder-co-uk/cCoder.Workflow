// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowInstanceDataControllerTests
{
    [Fact]
    public async Task Put_UpdatesFlowInstanceData()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(includeInstance: true);
        FlowInstanceData actualInstance;

        // When
        await UpdateFlowInstanceDataAsync(id:seededContext.InstanceId, payload:new
        {
            id = seededContext.InstanceId,
            flowDefinitionId = seededContext.FlowId,
            name = "Updated Instance",
            state = "Running",
            caller = "Guest",
            contextString = "{\"updated\":true}",
            start = DateTimeOffset.UtcNow,
        });

        actualInstance = await GetFlowInstanceDataAsync(id:seededContext.InstanceId);

        // Then
        actualInstance.Should().NotBeNull();
        actualInstance!.State.Should().Be(expected:"Running");

        await Teardown(seededContext:seededContext);
    }
}