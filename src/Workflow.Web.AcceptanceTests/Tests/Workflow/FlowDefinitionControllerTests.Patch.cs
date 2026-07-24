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
        string updatedName = Unique(prefix: "PatchedFlow");
        FlowDefinition actualFlowDefinition;

        // When
        await PatchFlowDefinitionAsync(flowDefinitionId: seededContext.FlowId, payload: new
        {
            name = updatedName,
            description = "Patched flow",
        });

        actualFlowDefinition = await GetFlowDefinitionAsync(flowDefinitionId: seededContext.FlowId);

        // Then
        actualFlowDefinition.Should()
            .NotBeNull();

        actualFlowDefinition!.Name.Should()
            .Be(expected: updatedName);

        await Teardown(seededContext: seededContext);
    }
}