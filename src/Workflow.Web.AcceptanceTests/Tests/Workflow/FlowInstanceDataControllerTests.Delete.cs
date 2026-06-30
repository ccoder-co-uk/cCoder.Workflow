using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowInstanceDataControllerTests
{
    [Fact]
    public async Task Delete_RemovesFlowInstanceData()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(includeInstance: true);
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteFlowInstanceDataAsync(seededContext.InstanceId);
        actualReadStatusCode = await GetFlowInstanceDataStatusCodeAsync(seededContext.InstanceId);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





