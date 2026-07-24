// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class BaselineTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<JsonElement> GetBaselineAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: "/Api/Workflow/Baseline");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return JsonDocument.Parse(json: content).RootElement.Clone();
    }
}