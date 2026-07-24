// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Extensions;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task Put_UpdatesFlowDefinition()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(includeFlow: true);
        string updatedName = Unique("UpdatedFlow");
        FlowDefinition actualFlowDefinition;

        // When
        await UpdateFlowDefinitionAsync(seededContext.FlowId, new
        {
            id = seededContext.FlowId,
            appId = seededContext.AppId,
            name = updatedName,
            description = "Updated flow",
            definitionJson = new Flow
            {
                Name = "Acceptance",
                Activities = [new Start { Ref = "start" }],
                Links = [],
            }.ToJson(),
            configJson = "{}",
        });

        actualFlowDefinition = await GetFlowDefinitionAsync(seededContext.FlowId);

        // Then
        actualFlowDefinition.Should().NotBeNull();
        actualFlowDefinition!.Name.Should().Be(updatedName);

        await Teardown(seededContext);
    }
}