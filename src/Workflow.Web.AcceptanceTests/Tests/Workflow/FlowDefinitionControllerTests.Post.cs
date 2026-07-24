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
    public async Task Post_CreatesFlowDefinition()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "Flow");
        FlowDefinition expectedFlowDefinition;
        FlowDefinition actualFlowDefinition;

        // When
        expectedFlowDefinition = await CreateFlowDefinitionAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance flow",
            definitionJson = new Flow
            {
                Name = "Acceptance",
                Activities = [new Start { Ref = "start" }],
                Links = [],
            }.ToJson(),
            configJson = "{}",
        });

        actualFlowDefinition = await GetFlowDefinitionAsync(flowDefinitionId: expectedFlowDefinition.Id);

        // Then
        actualFlowDefinition.Should().NotBeNull();
        actualFlowDefinition!.Name.Should().Be(expected: name);

        await DeleteFlowDefinitionAsync(flowDefinitionId: expectedFlowDefinition.Id);
        await Teardown(seededContext: seededContext);
    }
}