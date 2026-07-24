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
    public async Task Patch_UpdatesFlowInstanceData()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(includeInstance: true);
        FlowInstanceData actualInstance;

        // When
        await PatchFlowInstanceDataAsync(flowInstanceDataId: seededContext.InstanceId, payload: new
        {
            state = "Completed",
        });

        actualInstance = await GetFlowInstanceDataAsync(flowInstanceDataId: seededContext.InstanceId);

        // Then
        actualInstance.Should()
            .NotBeNull();

        actualInstance!.State.Should()
            .Be(expected: "Completed");

        await Teardown(seededContext: seededContext);
    }
}