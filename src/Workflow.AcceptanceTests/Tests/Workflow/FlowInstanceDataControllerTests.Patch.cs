using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowInstanceDataControllerTests
{
    [Fact]
    public async Task Patch_UpdatesFlowInstanceData()
    {
        // Given
        SeededFlowInstanceDataContext seededContext = await SeedDatabase(includeInstance: true);
        FlowInstanceData actualInstance;

        // When
        await PatchFlowInstanceDataAsync(seededContext.InstanceId, new
        {
            state = "Completed",
        });

        actualInstance = await GetFlowInstanceDataAsync(seededContext.InstanceId);

        // Then
        actualInstance.Should().NotBeNull();
        actualInstance!.State.Should().Be("Completed");

        await Teardown(seededContext);
    }
}





