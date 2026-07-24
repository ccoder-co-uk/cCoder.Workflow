// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class WorkflowEventControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetWorkflowEventCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfWorkflowEvents()
    {
        // Given

        // When
        IReadOnlyList<WorkflowEvent> actualWorkflowEvents = await GetWorkflowEventsAsync(top: 1);

        // Then
        actualWorkflowEvents.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsWorkflowEventById()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase(includeEvent: true);

        // When
        WorkflowEvent actualWorkflowEvent = await GetWorkflowEventAsync(workflowEventId: seededContext.EventId);

        // Then
        actualWorkflowEvent.Should().NotBeNull();
        actualWorkflowEvent.Id.Should().Be(expected: seededContext.EventId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededWorkflowEventContext seededContext = await SeedDatabase(
            includeEvent: true,
            "workflowevent_create",
            "workflowevent_update",
            "workflowevent_delete");

        int actualStatusCode = await GetWorkflowEventStatusCodeAsync(workflowEventId: seededContext.EventId);

        actualStatusCode.Should().Be(expected: (int)HttpStatusCode.NotFound);

        await Teardown(seededContext: seededContext);
    }
}