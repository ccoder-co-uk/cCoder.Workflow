// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using cCoder.Data.Models.Workflow;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;


namespace Workflow.Web;

public class Program
{
    private static ILogger log = null!;

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

        Config config = new();
        configuration.Bind(instance: config);
        builder.Services.AddSingleton(implementationInstance: config);
        builder.Services.AddEventing();

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

        string eventProviderType = ResolveEventProviderType(configuration: configuration);
        string httpEventHubUrl = HttpEventHubUrlResolver.Resolve(configuration: configuration);

        if (IsHttpEventProvider(eventProviderType: eventProviderType) && !string.IsNullOrWhiteSpace(value: httpEventHubUrl))
        {
            builder.Services.AddHttpEventingWeb(configure: options =>
            {
                options.HubUrl = httpEventHubUrl;
                options.MaxConcurrency = ResolveMaxConcurrency(configuration: configuration);
            });
        }

        builder.Services.AddWorkflowWeb(newConfigure: workflowConfig =>
        {
            if (IsHttpEventProvider(eventProviderType: eventProviderType) && !string.IsNullOrWhiteSpace(value: httpEventHubUrl))
            {
                workflowConfig.WithEventProviders(
eventProviders: CreateExternalSendProvider<FlowInstanceData>(["flow_instance_data_add"])
                );
            }
        });

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(url: "/swagger/Workflow/swagger.json", name: "Workflow API");
                options.SwaggerEndpoint(url: "/swagger/Core/swagger.json", name: "Core API");
                options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/", handler: () => Results.Redirect(url: "/tools/index.html"));
        app.StartWorkflowWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
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

    private static EventProvider<T> CreateExternalSendProvider<T>(string[] eventNames) =>
        new()
        {
            Events = eventNames,
            SendHandler = async (serviceProvider, eventName, message) =>
            {
                IHttpEventHub httpEventHub = serviceProvider.GetRequiredService<IHttpEventHub>();
                await httpEventHub.RaiseEventAsync(name: eventName, message: message);
            }
        };

    private static string ResolveEventProviderType(IConfiguration configuration) =>
        configuration.GetValue<string>(key: "Eventing:ProviderType")
        ?? configuration.GetValue<string>(key: "Eventing:Provider")
        ?? "Http";

    private static int ResolveMaxConcurrency(IConfiguration configuration) =>
        configuration.GetValue<int?>(key: "Eventing:Http:MaxConcurrency") ?? 1;

    private static bool IsHttpEventProvider(string eventProviderType) =>
        string.Equals(a: eventProviderType, b: "Http", comparisonType: StringComparison.OrdinalIgnoreCase);

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);

        await context.Response.WriteAsync(
text: "{ \"error\": \"" + exception.Message.Replace(oldValue: "\"", newValue: "\'") + "\" }");
    }
}