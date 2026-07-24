// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class WorkflowEventControllerTests
{
    [Fact]
    public async Task Put_UpdatesWorkflowEvent()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase(includeEvent: true);
        WorkflowEvent actualWorkflowEvent;

        // When
        await UpdateWorkflowEventAsync(seededContext.EventId, new
        {
            id = seededContext.EventId,
            flowId = seededContext.FlowId,
            type = "Updated",
            eventContext = "{\"updated\":true}",
            createdBy = "Guest",
            createdOn = DateTimeOffset.UtcNow,
            executeAs = "Guest",
        });

        actualWorkflowEvent = await GetWorkflowEventAsync(seededContext.EventId);

        // Then
        actualWorkflowEvent.Should().NotBeNull();
        actualWorkflowEvent!.Type.Should().Be("Updated");

        await Teardown(seededContext);
    }
}