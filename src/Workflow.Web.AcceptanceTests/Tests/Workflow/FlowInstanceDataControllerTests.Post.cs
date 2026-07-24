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
    public async Task Post_CreatesFlowInstanceData()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase();
        FlowInstanceData expectedInstance;
        FlowInstanceData actualInstance;

        // When
        expectedInstance = await CreateFlowInstanceDataAsync(payload: new
        {
            id = Guid.NewGuid(),
            flowDefinitionId = seededContext.FlowId,
            name = Unique(prefix: "Instance"),
            state = "Queued",
            caller = "Guest",
            contextString = "{}",
            start = DateTimeOffset.UtcNow,
        });

        actualInstance = await GetFlowInstanceDataAsync(id: expectedInstance.Id);

        // Then
        actualInstance.Should().NotBeNull();
        actualInstance!.Id.Should().Be(expected: expectedInstance.Id);

        await DeleteFlowInstanceDataAsync(id: expectedInstance.Id);
        await Teardown(seededContext: seededContext);
    }
}