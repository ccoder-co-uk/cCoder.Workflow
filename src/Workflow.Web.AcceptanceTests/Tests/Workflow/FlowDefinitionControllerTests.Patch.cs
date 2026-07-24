// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task Patch_UpdatesFlowDefinition()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(includeFlow: true);
        string updatedName = Unique("PatchedFlow");
        FlowDefinition actualFlowDefinition;

        // When
        await PatchFlowDefinitionAsync(seededContext.FlowId, new
        {
            name = updatedName,
            description = "Patched flow",
        });

        actualFlowDefinition = await GetFlowDefinitionAsync(seededContext.FlowId);

        // Then
        actualFlowDefinition.Should().NotBeNull();
        actualFlowDefinition!.Name.Should().Be(updatedName);

        await Teardown(seededContext);
    }
}