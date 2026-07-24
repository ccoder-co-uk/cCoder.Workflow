// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        int actualStatusCode = await DeleteFlowDefinitionAsync(flowDefinitionId: seededContext.FlowId);
        actualReadStatusCode = await GetFlowDefinitionStatusCodeAsync(flowDefinitionId: seededContext.FlowId);

        // Then
        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}