// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.HostedServices;

[Collection(HostedServicesAcceptanceCollection.Name)]
public sealed partial class HealthControllerTests(HostedServicesAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private Task<string> GetHealthAsync() =>
        Client.GetStringAsync(requestUri: "Health");

    private Task<string> GetHomeAsync() =>
        Client.GetStringAsync(requestUri: "/");
}