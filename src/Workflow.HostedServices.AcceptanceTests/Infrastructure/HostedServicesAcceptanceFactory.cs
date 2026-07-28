// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Workflow.Dependencies.HostedServices;
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
                new KeyValuePair<string, string>(
                    key: "Data:ConnectionString",
                    value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(
                    key: "Workflow:ConnectionString",
                    value: settings.CoreConnectionString),
                new KeyValuePair<string, string>(
                    key: "Security:ConnectionString",
                    value: settings.SsoConnectionString),
                new KeyValuePair<string, string>(
                    key: "Security:DecryptionKey",
                    value: settings.DecryptionKey),
                new KeyValuePair<string, string>(
                    key: "Eventing:Http:HubUrl",
                    value: string.Empty),
                new KeyValuePair<string, string>(
                    key: "Workflow:IsMigrating",
                    value: "true"),
            ]);
        });

        builder.ConfigureTestServices(servicesConfiguration: services =>
        {
            services.RemoveAll<ICoreContextFactory>();
            services.RemoveAll<DataConfiguration>();
            services.RemoveAll<ISecurityDbContextFactory>();
            services.RemoveAll<IInstanceMaintenanceBackgroundServiceDependency>();
            services.RemoveAll<IQueueInstanceBackgroundServiceDependency>();
            services.RemoveAll<IScheduledTaskRunnerBackgroundServiceDependency>();

            ServiceDescriptor[] hostedWorkflowServices = services
                .Where(predicate: descriptor =>
                    descriptor.ServiceType == typeof(IHostedService)
                    && descriptor.ImplementationFactory is not null)
                .ToArray();

            foreach (ServiceDescriptor descriptor in hostedWorkflowServices)
            {
                services.Remove(item: descriptor);
            }

            services.AddSingleton<ISecurityDbContextFactory>(
                implementationFactory: _ =>
                    new MSSQLSecurityDbContextFactory(
                        connectionString: settings.SsoConnectionString));

            services.AddData(
                configuration: new DataConfiguration
                {
                    ConnectionString = settings.CoreConnectionString
                });
        });
    }
}