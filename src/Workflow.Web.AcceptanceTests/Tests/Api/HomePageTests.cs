// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class HomePageTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<HttpResponseMessage> GetHomeAsync() =>
        await Client.GetAsync("/");

    private async Task<string> GetToolsAsync() =>
        await Client.GetStringAsync("/tools/index.html");
}