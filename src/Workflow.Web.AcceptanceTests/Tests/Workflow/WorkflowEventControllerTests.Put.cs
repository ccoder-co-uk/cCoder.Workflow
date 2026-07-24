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
        await UpdateWorkflowEventAsync(workflowEventId: seededContext.EventId, payload: new
        {
            id = seededContext.EventId,
            flowId = seededContext.FlowId,
            type = "Updated",
            eventContext = "{\"updated\":true}",
            createdBy = "Guest",
            createdOn = DateTimeOffset.UtcNow,
            executeAs = "Guest",
        });

        actualWorkflowEvent = await GetWorkflowEventAsync(workflowEventId: seededContext.EventId);

        // Then
        actualWorkflowEvent.Should()
            .NotBeNull();
        actualWorkflowEvent!.Type.Should()
            .Be(expected: "Updated");

        await Teardown(seededContext: seededContext);
    }
}