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

    private Task<HttpResponseMessage> GetHomeAsync() =>
        Client.GetAsync(requestUri: "/");

    private Task<string> GetToolsAsync() =>
        Client.GetStringAsync(requestUri: "/tools/index.html");
}