// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFlowDefinitionCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFlowDefinitions()
    {
        // Given

        // When
        IReadOnlyList<FlowDefinition> actualFlowDefinitions = await GetFlowDefinitionsAsync(top: 1);

        // Then
        actualFlowDefinitions.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFlowDefinitionById()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(includeFlow: true);

        // When
        FlowDefinition actualFlowDefinition = await GetFlowDefinitionAsync(flowDefinitionId: seededContext.FlowId);

        // Then
        actualFlowDefinition.Should()
            .NotBeNull();

        actualFlowDefinition.Id.Should()
            .Be(expected: seededContext.FlowId);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(
            includeFlow: true,
            privileges:
            [
                "flowdefinition_create",
                "flowdefinition_update",
                "flowdefinition_execute",
                "flowdefinition_delete"
            ]);

        // When
        int actualStatusCode = await GetFlowDefinitionStatusCodeAsync(flowDefinitionId: seededContext.FlowId);

        // Then
        actualStatusCode.Should()
            .Be(expected: (int)HttpStatusCode.NotFound);

        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task KnownActivityTypes_ReturnsTypes()
    {
        // Given

        // When
        string actualContent = await GetKnownActivityTypesAsync();

        // Then
        actualContent.Should()
            .Contain(expected: "Start");
    }

    [Fact]
    public async Task KnownSystemTypes_ReturnsTypes()
    {
        // Given

        // When
        string actualContent = await GetKnownSystemTypesAsync();

        // Then
        actualContent.Should()
            .Contain(expected: "System");
    }
}