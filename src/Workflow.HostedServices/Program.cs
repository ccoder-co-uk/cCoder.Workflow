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
using cCoder.Workflow.Services.Orchestrations;

namespace Workflow.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        IConfiguration configuration = ConfigureApplication(configuration: builder.Configuration, environment: builder.Environment);

        string coreConnection = GetRequiredConfigurationValue(
configuration: configuration,
key: "ConnectionStrings:Core");

        string ssoConnection = GetRequiredConfigurationValue(
configuration: configuration,
key: "ConnectionStrings:SSO");

        builder.Services.AddEventing();

        builder.Services.AddHttpEventingHostedServices(configure: options =>
        {
            options.MaxConcurrency = ResolveMaxConcurrency(configuration: configuration);
        });

        builder.Services.AddControllers();

        builder.Services.AddSecurityApi(configAction: (services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services: services, connectionString: ssoConnection);

            securityConfig.UseAESHMMACPasswordEncryption(
services: services,
decryptionKey: GetRequiredConfigurationValue(configuration, "Settings:DecryptionKey"));
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
services: builder.Services,
connectionString: coreConnection);

        builder.Services.AddWorkflowHostedServices(configure: workflowConfiguration =>
        {
            workflowConfiguration.IsMigrating =
                configuration.GetValue<int?>(key: "MIGRATING") == 1
                || configuration.GetValue<bool?>(key: "Workflow:IsMigrating") == true;

            workflowConfiguration.WithEventProviders(
                CreateAppReceiveProvider(),
                CreateQueuedFlowInstanceReceiveProvider());
        });

        WebApplication app = builder.Build();
        app.UseSession();
        app.MapControllers();
        app.StartWorkflowHostedServices();
        app.Run();
    }

    private static IConfiguration ConfigureApplication(
        ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        configuration
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(path: $"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
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

    private static int ResolveMaxConcurrency(IConfiguration configuration) =>
        configuration.GetValue<int?>(key: "Eventing:Http:MaxConcurrency") ?? 1;

    private static EventProvider<App> CreateAppReceiveProvider() =>
        new()
        {
            Events = ["app_add", "app_update", "app_delete"],
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

                await eventHub.RaiseEventAsync(
name: eventName,
message: new EventMessage<App>
{
    AuthInfo = new EventAuthInfo
    {
        SSOUserId = message.AuthInfo?.SSOUserId ?? "Guest",
    },
    Data = message.Data,
});
            },
        };

    private static EventProvider<FlowInstanceData> CreateQueuedFlowInstanceReceiveProvider() =>
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

                if (!string.Equals(a: message.Data?.State, b: "Queued", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                IWorkflowInstanceManagementOrchestrationService workflowInstanceManagementService =
                    serviceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();

                await workflowInstanceManagementService.ExecuteWaitingQueuedInstanceByIdAsync(
flowInstanceDataId: message.Data.Id);
            },
        };
}