// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.AcceptanceTests.Models;


namespace Web.AcceptanceTests.Infrastructure;

internal sealed class WebAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Workflow.Web.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");

        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>(
                    key: "ConnectionStrings:Core",
                    value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(
                    key: "ConnectionStrings:SSO",
                    value: settings.SsoConnectionString),
                new KeyValuePair<string, string>(
                    key: "Settings:DecryptionKey",
                    value: settings.DecryptionKey),
                new KeyValuePair<string, string>(
                    key: "Settings:enableExternalEventing",
                    value: "false"),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();

            services.AddSingleton(
implementationInstance: new cCoder.Data.Config
{
    ConnectionStrings = new Dictionary<string, string>
    {
        ["Core"] = settings.CoreConnectionString,
        ["SSO"] = settings.SsoConnectionString,
    },
    Settings = new Dictionary<string, string>
    {
        ["DecryptionKey"] = settings.DecryptionKey,
        ["enableExternalEventing"] = "false",
    },
    Services = new Dictionary<string, string>(),
});

            services.AddSingleton<ISecurityDbContextFactory>(
                implementationFactory: _ =>
                    new MSSQLSecurityDbContextFactory(
                        connectionString: settings.SsoConnectionString)
            );

            services.AddCoreData(connectionString: settings.CoreConnectionString);
        });
    }
}