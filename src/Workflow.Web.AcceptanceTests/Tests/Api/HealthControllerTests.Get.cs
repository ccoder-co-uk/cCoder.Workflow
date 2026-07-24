// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class HealthControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Given

        // When
        string actualHealth = await GetHealthAsync();

        // Then
        actualHealth.Should().Be(expected: "OK");
    }
}