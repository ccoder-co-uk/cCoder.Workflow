using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class WorkflowEventControllerTests
{
    [Fact]
    public async Task Delete_RemovesWorkflowEvent()
    {
        // Given
        SeededWorkflowEventContext seededContext = await SeedDatabase(includeEvent: true);
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteWorkflowEventAsync(seededContext.EventId);
        actualReadStatusCode = await GetWorkflowEventStatusCodeAsync(seededContext.EventId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





