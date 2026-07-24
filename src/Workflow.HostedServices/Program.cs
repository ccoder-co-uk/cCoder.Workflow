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
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = ConfigureApplication(builder.Configuration, builder.Environment);

        string coreConnection = GetRequiredConfigurationValue(
            configuration,
            "ConnectionStrings:Core");

        string ssoConnection = GetRequiredConfigurationValue(
            configuration,
            "ConnectionStrings:SSO");

        builder.Services.AddEventing();
        builder.Services.AddHttpEventingHostedServices(options =>
        {
            options.MaxConcurrency = ResolveMaxConcurrency(configuration);
        });
        builder.Services.AddControllers();

        builder.Services.AddSecurityApi((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                GetRequiredConfigurationValue(configuration, "Settings:DecryptionKey"));
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            builder.Services,
            coreConnection);

        builder.Services.AddWorkflowHostedServices(workflowConfiguration =>
        {
            workflowConfiguration.IsMigrating =
                configuration.GetValue<int?>("MIGRATING") == 1
                || configuration.GetValue<bool?>("Workflow:IsMigrating") == true;

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
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return configuration;
    }

    private static string GetRequiredConfigurationValue(
        IConfiguration configuration,
        string key)
    {
        string value = configuration.GetValue<string>(key);

        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"{key} is required.")
            : value;
    }

    private static int ResolveMaxConcurrency(IConfiguration configuration) =>
        configuration.GetValue<int?>("Eventing:Http:MaxConcurrency") ?? 1;

    private static EventProvider<App> CreateAppReceiveProvider() =>
        new()
        {
            Events = ["app_add", "app_update", "app_delete"],
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

                await eventHub.RaiseEventAsync(
                    eventName,
                    new EventMessage<App>
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
                    throw new InvalidOperationException(
                        "You must provide a workflow instance payload with a valid id.");

                if (!string.Equals(message.Data?.State, "Queued", StringComparison.OrdinalIgnoreCase))
                    return;

                IWorkflowInstanceManagementOrchestrationService workflowInstanceManagementService =
                    serviceProvider.GetRequiredService<IWorkflowInstanceManagementOrchestrationService>();

                await workflowInstanceManagementService.ExecuteWaitingQueuedInstanceByIdAsync(
                    message.Data.Id);
            },
        };
}