// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Workflow;

public sealed partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task Execute_QueuesFlowInstance()
    {
        // Given
        SeededFlowDefinitionContext seededContext = await SeedDatabase(includeFlow: true);

        // When
        int actualStatusCode = await ExecuteFlowDefinitionAsync(flowDefinitionId: seededContext.FlowId, payload: "{}");

        // Then
        actualStatusCode.Should()
            .Be(expected: 200);

        await Teardown(seededContext: seededContext);
    }
}