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
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = ConfigureApplication(builder.Configuration, builder.Environment);

        string coreConnection = GetRequiredConfigurationValue(
            configuration,
            "ConnectionStrings:Core");

        string ssoConnection = GetRequiredConfigurationValue(
            configuration,
            "ConnectionStrings:SSO");

        Config config = new();
        configuration.Bind(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddEventing();

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

        string eventProviderType = ResolveEventProviderType(configuration);
        string httpEventHubUrl = HttpEventHubUrlResolver.Resolve(configuration);

        if (IsHttpEventProvider(eventProviderType) && !string.IsNullOrWhiteSpace(httpEventHubUrl))
        {
            builder.Services.AddHttpEventingWeb(options =>
            {
                options.HubUrl = httpEventHubUrl;
                options.MaxConcurrency = ResolveMaxConcurrency(configuration);
            });
        }

        builder.Services.AddWorkflowWeb(workflowConfig =>
        {
            if (IsHttpEventProvider(eventProviderType) && !string.IsNullOrWhiteSpace(httpEventHubUrl))
            {
                workflowConfig.WithEventProviders(
                    CreateExternalSendProvider<FlowInstanceData>(["flow_instance_data_add"])
                );
            }
        });

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Workflow/swagger.json", "Workflow API");
                options.SwaggerEndpoint("/swagger/Core/swagger.json", "Core API");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.StartWorkflowWeb(log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(HandleUnhandledException);
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

    private static EventProvider<T> CreateExternalSendProvider<T>(string[] eventNames) =>
        new()
        {
            Events = eventNames,
            SendHandler = async (serviceProvider, eventName, message) =>
            {
                IHttpEventHub httpEventHub = serviceProvider.GetRequiredService<IHttpEventHub>();
                await httpEventHub.RaiseEventAsync(eventName, message);
            }
        };

    private static string ResolveEventProviderType(IConfiguration configuration) =>
        configuration.GetValue<string>("Eventing:ProviderType")
        ?? configuration.GetValue<string>("Eventing:Provider")
        ?? "Http";

    private static int ResolveMaxConcurrency(IConfiguration configuration) =>
        configuration.GetValue<int?>("Eventing:Http:MaxConcurrency") ?? 1;

    private static bool IsHttpEventProvider(string eventProviderType) =>
        string.Equals(eventProviderType, "Http", StringComparison.OrdinalIgnoreCase);

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;
        context.Response.ContentType = "application/json";

        if (exception is null)
            return;

        log.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
        await context.Response.WriteAsync(
            "{ \"error\": \"" + exception.Message.Replace("\"", "\'") + "\" }");
    }
}



