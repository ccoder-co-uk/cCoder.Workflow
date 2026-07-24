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
    public async Task Post_CreatesWorkflowEvent()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase();
        WorkflowEvent expectedWorkflowEvent;
        WorkflowEvent actualWorkflowEvent;

        // When
        expectedWorkflowEvent = await CreateWorkflowEventAsync(payload: new
        {
            flowId = seededContext.FlowId,
            type = "Acceptance",
            eventContext = "{}",
            createdBy = "Guest",
            createdOn = DateTimeOffset.UtcNow,
            executeAs = "Guest",
        });

        actualWorkflowEvent = await GetWorkflowEventAsync(workflowEventId: expectedWorkflowEvent.Id);

        // Then
        actualWorkflowEvent.Should().NotBeNull();
        actualWorkflowEvent!.Id.Should().Be(expected: expectedWorkflowEvent.Id);

        await DeleteWorkflowEventAsync(workflowEventId: expectedWorkflowEvent.Id);
        await Teardown(seededContext: seededContext);
    }
}