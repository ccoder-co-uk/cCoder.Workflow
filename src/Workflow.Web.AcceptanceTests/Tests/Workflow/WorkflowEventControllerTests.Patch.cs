using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class WorkflowEventControllerTests
{
    [Fact]
    public async Task Patch_UpdatesWorkflowEvent()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase(includeEvent: true);
        WorkflowEvent actualWorkflowEvent;

        // When
        await PatchWorkflowEventAsync(seededContext.EventId, new
        {
            type = "Patched",
        });

        actualWorkflowEvent = await GetWorkflowEventAsync(seededContext.EventId);

        // Then
        actualWorkflowEvent.Should().NotBeNull();
        actualWorkflowEvent!.Type.Should().Be("Patched");

        await Teardown(seededContext);
    }
}





