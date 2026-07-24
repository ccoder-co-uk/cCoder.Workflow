// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Workflow;
using cCoder.Workflow.Services.Processings;
using Workflow.HostedServices.Services.Processings;

namespace Workflow.HostedServices.Extensions;

internal static class WebApplicationBuilderExtensions
{
    internal static void AddWorkflowHostedServicesApplication(
        this IServiceCollection services,
        ConfigurationManager configurationManager,
        IWebHostEnvironment environment)
    {
        IConfiguration configuration = ConfigureApplication(
            configuration: configurationManager,
            environment: environment);

        string coreConnection = GetRequiredConfigurationValue(
            configuration: configuration,
            key: "ConnectionStrings:Core");

        string ssoConnection = GetRequiredConfigurationValue(
            configuration: configuration,
            key: "ConnectionStrings:SSO");

        services.AddEventing();
        services.AddSingleton<IHealthProcessingService, HealthProcessingService>();
        services.AddSingleton<IHomeProcessingService, HomeProcessingService>();

        services.AddHttpEventingHostedServices(configure: options =>
        {
            options.MaxConcurrency = ResolveMaxConcurrency(
                configuration: configuration);
        });

        services.AddControllers();

        services.AddSecurityApi(configAction: (serviceCollection, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(
                services: serviceCollection,
                connectionString: ssoConnection);

            string decryptionKey = GetRequiredConfigurationValue(
                configuration: configuration,
                key: "Settings:DecryptionKey");

            securityConfig.UseAESHMMACPasswordEncryption(
                services: services,
                decryptionKey: decryptionKey);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            services: services,
            connectionString: coreConnection);

        services.AddWorkflowHostedServices(
            newConfigure: workflowConfiguration =>
            {
                workflowConfiguration.IsMigrating =
                    configuration.GetValue<int?>(key: "MIGRATING") == 1
                    || configuration.GetValue<bool?>(
                        key: "Workflow:IsMigrating") == true;

                workflowConfiguration.EventProviders =
                [
                    CreateAppReceiveProvider(),
                    CreateQueuedFlowInstanceReceiveProvider()
                ];
            });
    }

    internal static void UseWorkflowHostedServicesApplication(
        this WebApplication app)
    {
        app.UseSession();
        app.MapControllers();
        app.StartWorkflowHostedServices();
    }

    private static IConfiguration ConfigureApplication(
        ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        configuration
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(
                path: "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .AddJsonFile(
                path: $"appsettings.{environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();

        return configuration;
    }

    private static string GetRequiredConfigurationValue(
        IConfiguration configuration,
        string key)
    {
        string value = configuration.GetValue<string>(key: key);

        return string.IsNullOrWhiteSpace(value: value)
            ? throw new InvalidOperationException($"{key} is required.")
            : value;
    }

    private static int ResolveMaxConcurrency(
        IConfiguration configuration) =>
        configuration.GetValue<int?>(
            key: "Eventing:Http:MaxConcurrency") ?? 1;

    private static EventProvider<App> CreateAppReceiveProvider() =>
        new()
        {
            Events = ["app_add", "app_update", "app_delete"],
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub =
                    serviceProvider.GetRequiredService<IEventHub>();

                EventMessage<App> eventMessage = new()
                {
                    AuthInfo = new EventAuthInfo
                    {
                        SSOUserId = message.AuthInfo?.SSOUserId ?? "Guest",
                    },
                    Data = message.Data,
                };

                await eventHub.RaiseEventAsync(
                    name: eventName,
                    message: eventMessage);
            },
        };

    private static EventProvider<FlowInstanceData>
        CreateQueuedFlowInstanceReceiveProvider() =>
        new()
        {
            Events = ["flow_instance_data_add"],
            ReceiveHandler = async (serviceProvider, _, message) =>
            {
                if (message.Data?.Id == Guid.Empty)
                {
                    throw new InvalidOperationException(
                        "You must provide a workflow instance payload with a valid id.");
                }

                if (!string.Equals(
                    a: message.Data?.State,
                    b: "Queued",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                IWorkflowInstanceProcessingService
                    workflowInstanceProcessingService =
                    serviceProvider
                        .GetRequiredService<IWorkflowInstanceProcessingService>();

                await workflowInstanceProcessingService
                    .ExecuteWaitingQueuedInstanceByIdAsync(
                        flowInstanceDataId: message.Data.Id);
            },
        };
}