// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Workflow.Exposures.HostedServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Web.AcceptanceTests.Models;

namespace Web.AcceptanceTests.Infrastructure;

internal sealed class HostedServicesAcceptanceFactory(AcceptanceSettings settings)
    : WebApplicationFactory<Workflow.HostedServices.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment: "Acceptance");
        builder.ConfigureAppConfiguration(configureDelegate: (_, config) =>
        {
            config.AddInMemoryCollection(
initialData: [
                new KeyValuePair<string, string>("ConnectionStrings:Core", settings.CoreConnectionString),
                new KeyValuePair<string, string>("ConnectionStrings:SSO", settings.SsoConnectionString),
                new KeyValuePair<string, string>("Settings:DecryptionKey", settings.DecryptionKey),
                new KeyValuePair<string, string>("Settings:enableExternalEventing", "false"),
                new KeyValuePair<string, string>("Workflow:IsMigrating", "true"),
            ]);
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<IInstanceMaintenanceManagement>();
            services.RemoveAll<IQueueInstanceManagement>();
            services.RemoveAll<IScheduledTaskRunnerManagement>();

            ServiceDescriptor[] hostedWorkflowServices = services
                .Where(predicate: descriptor =>
                    descriptor.ServiceType == typeof(IHostedService)
                    && descriptor.ImplementationFactory is not null)
                .ToArray();

            foreach (ServiceDescriptor descriptor in hostedWorkflowServices)
            {
                services.Remove(item: descriptor);
            }

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
                _ => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString)
            );
            services.AddCoreData(connectionString: settings.CoreConnectionString);
        });
    }
}