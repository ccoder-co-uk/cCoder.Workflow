using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Security;
using cCoder.Security.Api;
using cCoder.Security.Data.EF.MSSQL;
using cCoder.Security.Objects;
using cCoder.Workflow;
using cCoder.Eventing.Http.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

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
        builder.Services.AddControllers()
            .ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(
                    new ExcludeHttpEventControllerFeatureProvider(typeof(HttpEventController))));
        builder.Services.AddScoped<ReceivedHttpEventProcessor>();

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

        builder.Services.AddWorkflowHostedServices();

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
            .AddJsonFile("appsettings.testing.json", optional: true, reloadOnChange: true)
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

    private sealed class ExcludeHttpEventControllerFeatureProvider(Type controllerType)
        : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(
            IEnumerable<ApplicationPart> parts,
            ControllerFeature feature)
        {
            for (int index = feature.Controllers.Count - 1; index >= 0; index--)
            {
                if (feature.Controllers[index].AsType() == controllerType)
                    feature.Controllers.RemoveAt(index);
            }
        }
    }
}
