using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task Delete_RemovesFlowDefinition()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(includeFlow: true);
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteFlowDefinitionAsync(seededContext.FlowId);
        actualReadStatusCode = await GetFlowDefinitionStatusCodeAsync(seededContext.FlowId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





