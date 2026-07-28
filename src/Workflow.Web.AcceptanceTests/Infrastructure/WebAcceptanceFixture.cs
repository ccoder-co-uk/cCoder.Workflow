// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.AcceptanceTests.Models;
using Xunit;


namespace Web.AcceptanceTests.Infrastructure;

public sealed class WebAcceptanceFixture : IAsyncLifetime
{
    private AcceptanceDatabaseManager databaseManager;

    internal WebAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        AcceptanceSettings settings = new()
        {
            CoreConnectionString = configuration.CoreConnectionString,
            SsoConnectionString = configuration.SecurityConnectionString,
            DecryptionKey = configuration.SecurityDecryptionKey
        };

        Factory = new WebAcceptanceFactory(settings);
        databaseManager = new AcceptanceDatabaseManager(Factory.Services);
        await databaseManager.ResetDatabasesAsync();
        await SeedAsync();

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (databaseManager is not null)
        {
            await databaseManager.DropDatabasesAsync();
        }

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    private Task SeedAsync() =>
        new AcceptanceApplicationSeeder(Factory.Services).SeedAsync();

}

[CollectionDefinition(Name)]
public sealed class WebAcceptanceCollection : ICollectionFixture<WebAcceptanceFixture>
{
    public const string Name = "Web acceptance";
}