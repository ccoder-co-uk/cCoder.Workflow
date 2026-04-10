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
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfWorkflowEvents()
    {
        // Given

        // When
        IReadOnlyList<WorkflowEvent> actualWorkflowEvents = await GetWorkflowEventsAsync(1);

        // Then
        actualWorkflowEvents.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsWorkflowEventById()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase(includeEvent: true);

        // When
        WorkflowEvent actualWorkflowEvent = await GetWorkflowEventAsync(seededContext.EventId);

        // Then
        actualWorkflowEvent.Should().NotBeNull();
        actualWorkflowEvent.Id.Should().Be(seededContext.EventId);

        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededWorkflowEventContext seededContext = await SeedDatabase(
            includeEvent: true,
            "workflowevent_create",
            "workflowevent_update",
            "workflowevent_delete");

        int actualStatusCode = await GetWorkflowEventStatusCodeAsync(seededContext.EventId);

        actualStatusCode.Should().Be((int)HttpStatusCode.NotFound);

        await Teardown(seededContext);
    }
}





