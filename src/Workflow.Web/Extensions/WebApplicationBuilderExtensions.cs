// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Workflow;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;
using Workflow.Web.Services.Processings;

namespace Workflow.Web.Extensions;

internal static class WebApplicationBuilderExtensions
{
    private static ILogger log = null!;

    internal static void AddWorkflowWebApplication(
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

        Config config = new();
        configuration.Bind(instance: config);
        services.AddSingleton(implementationInstance: config);
        services.AddScoped<ICoreAppProcessingService, CoreAppProcessingService>();
        services.AddScoped<ICoreUserProcessingService, CoreUserProcessingService>();
        services.AddSingleton<IHealthProcessingService, HealthProcessingService>();
        services.AddEventing();

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

        string eventProviderType = ResolveEventProviderType(
            configuration: configuration);

        string httpEventHubUrl = HttpEventHubUrlResolver.Resolve(
            configuration: configuration);

        if (IsHttpEventProvider(eventProviderType: eventProviderType)
            && !string.IsNullOrWhiteSpace(value: httpEventHubUrl))
        {
            services.AddHttpEventingWeb(configure: options =>
            {
                options.HubUrl = httpEventHubUrl;
                options.MaxConcurrency = ResolveMaxConcurrency(
                    configuration: configuration);
            });
        }

        services.AddWorkflowWeb(newConfigure: workflowConfig =>
        {
            if (IsHttpEventProvider(eventProviderType: eventProviderType)
                && !string.IsNullOrWhiteSpace(value: httpEventHubUrl))
            {
                workflowConfig.EventProviders =
                [
                    CreateExternalSendProvider<FlowInstanceData>(
                        eventNames: ["flow_instance_data_add"])
                ];
            }
        });
    }

    internal static void UseWorkflowWebApplication(
        this WebApplication app)
    {
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(
                    url: "/swagger/Workflow/swagger.json",
                    name: "Workflow API");

                options.SwaggerEndpoint(
                    url: "/swagger/Core/swagger.json",
                    name: "Core API");

                options.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(
            pattern: "/",
            handler: () => Results.Redirect(url: "/tools/index.html"));

        app.StartWorkflowWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
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

    private static EventProvider<T> CreateExternalSendProvider<T>(
        string[] eventNames) =>
        new()
        {
            Events = eventNames,
            SendHandler = async (serviceProvider, eventName, message) =>
            {
                IHttpEventHub httpEventHub =
                    serviceProvider.GetRequiredService<IHttpEventHub>();

                await httpEventHub.RaiseEventAsync(
                    name: eventName,
                    message: message);
            }
        };

    private static string ResolveEventProviderType(
        IConfiguration configuration) =>
        configuration.GetValue<string>(key: "Eventing:ProviderType")
        ?? configuration.GetValue<string>(key: "Eventing:Provider")
        ?? "Http";

    private static int ResolveMaxConcurrency(
        IConfiguration configuration) =>
        configuration.GetValue<int?>(
            key: "Eventing:Http:MaxConcurrency") ?? 1;

    private static bool IsHttpEventProvider(
        string eventProviderType) =>
        string.Equals(
            a: eventProviderType,
            b: "Http",
            comparisonType: StringComparison.OrdinalIgnoreCase);

    private static async Task HandleUnhandledException(
        HttpContext context)
    {
        Exception exception =
            context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError(
            message: "{Message}\n{StackTrace}",
            exception.Message,
            exception.StackTrace);

        string errorMessage =
            exception.Message.Replace(oldValue: "\"", newValue: "\'");

        await context.Response.WriteAsync(
            text: "{ \"error\": \"" + errorMessage + "\" }");
    }
}