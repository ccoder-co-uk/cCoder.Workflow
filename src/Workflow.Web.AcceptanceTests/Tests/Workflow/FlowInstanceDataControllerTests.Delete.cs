// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        int actualStatusCode = await DeleteFlowInstanceDataAsync(flowInstanceDataId: seededContext.InstanceId);
        actualReadStatusCode = await GetFlowInstanceDataStatusCodeAsync(flowInstanceDataId: seededContext.InstanceId);

        // Then
        actualStatusCode.Should().Be(expected: 200);
        actualReadStatusCode.Should().Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}