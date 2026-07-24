// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class BaselineTests
{
    [Fact]
    public async Task Get_GivenBaselineEndpoint_ShouldReturnPackagesArray()
    {
        JsonElement baseline = await GetBaselineAsync();

        baseline.ValueKind.Should().Be(JsonValueKind.Array);
    }
}