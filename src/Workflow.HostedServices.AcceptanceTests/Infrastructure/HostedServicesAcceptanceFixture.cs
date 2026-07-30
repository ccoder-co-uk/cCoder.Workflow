// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.AcceptanceTests.Models;
using Xunit;

namespace Web.AcceptanceTests.Infrastructure;

public sealed class HostedServicesAcceptanceFixture : IAsyncLifetime
{
    private AcceptanceDatabaseManager databaseManager;

    internal HostedServicesAcceptanceFactory Factory { get; private set; } = null!;

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

        Factory = new HostedServicesAcceptanceFactory(settings);
        databaseManager = new AcceptanceDatabaseManager(Factory.Services);
        await databaseManager.ResetDatabasesAsync();

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

}