using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.Workflow;

public static partial class IServiceCollectionExtensions
{
    private static WorkflowConfiguration AddConfiguredWorkflow(
        this IServiceCollection services,
        Action<IServiceCollection, WorkflowConfiguration> configure)
    {
        WorkflowConfiguration configuration = CreateConfiguration(services, configure);
        services.AddWorkflow();
        return configuration;
    }

    private static WorkflowConfiguration AddConfiguredWorkflowWeb(
        this IServiceCollection services,
        Action<IServiceCollection, WorkflowConfiguration> configure,
        ODataConventionModelBuilder builder = null)
    {
        WorkflowConfiguration configuration = CreateConfiguration(services, configure);
        services.AddWorkflowWeb(builder);
        services.AddConfiguredApi(
            configuration,
            "Workflow",
            static modelBuilder => modelBuilder.ConfigureWorkflowApiModel(),
            builder);

        return configuration;
    }

    private static WorkflowConfiguration AddConfiguredWorkflowHostedServices(
        this IServiceCollection services,
        Action<IServiceCollection, WorkflowConfiguration> configure)
    {
        WorkflowConfiguration configuration = CreateConfiguration(services, configure);
        services.AddWorkflowHostedServices();
        return configuration;
    }

    public static void ConfigureWorkflowApiModel(this ODataConventionModelBuilder builder) =>
        new WorkflowModelBuilder(builder).Configure();

    private static WorkflowConfiguration CreateConfiguration(
        IServiceCollection services,
        Action<IServiceCollection, WorkflowConfiguration> configure)
    {
        WorkflowConfiguration configuration = new();
        configure?.Invoke(services, configuration);
        PopulateBlankValuesFromEnvironment(configuration);
        services.AddSingleton(configuration);
        services.AddEventProviders(configuration.EventProviders);
        return configuration;
    }

    private static void PopulateBlankValuesFromEnvironment(WorkflowConfiguration configuration)
    {
        PopulateBlankValuesFromEnvironment("ConnectionStrings", configuration.ConnectionStrings);
        PopulateBlankValuesFromEnvironment("Settings", configuration.Settings);
        PopulateBlankValuesFromEnvironment("Services", configuration.Services);
    }

    private static void PopulateBlankValuesFromEnvironment(
        string section,
        IDictionary<string, string> values)
    {
        if (values is null)
            return;

        foreach (string key in values.Keys.ToArray())
        {
            if (!string.IsNullOrWhiteSpace(values[key]))
                continue;

            string environmentValue = GetEnvironmentConfigurationValue(section, key);

            if (!string.IsNullOrWhiteSpace(environmentValue))
                values[key] = environmentValue;
        }
    }

    private static string GetEnvironmentConfigurationValue(string section, string key) =>
        Environment.GetEnvironmentVariable($"{section}__{key}")
        ?? Environment.GetEnvironmentVariable($"{section}:{key}")
        ?? Environment.GetEnvironmentVariable($"ENV_{section}__{key}")
        ?? Environment.GetEnvironmentVariable($"ENV_{section}:{key}");

    private static void AddConfiguredApi(
        this IServiceCollection services,
        WorkflowConfiguration configuration,
        string documentName,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, documentName, configuration, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();
        string rootPath = string.IsNullOrWhiteSpace(configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        services.AddControllers().AddOData(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(1000)
                .AddRouteComponents(rootPath, routeModel, batchHandler);

            if (builder is null
                && configuration.IncludeLegacyCoreContext
                && !string.Equals(rootPath, "Api/Core", StringComparison.OrdinalIgnoreCase))
            {
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
            }
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string documentName,
        WorkflowConfiguration configuration,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, documentName, configuration);
            options.DocInclusionPredicate(
                (swaggerDocumentName, apiDescription) =>
                    ShouldIncludeInDocument(
                        swaggerDocumentName,
                        apiDescription.RelativePath,
                        documentName,
                        configuration));

            if (useFullSchemaIds)
                options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string documentName,
        WorkflowConfiguration configuration)
    {
        options.SwaggerDoc(documentName, new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });

        if (configuration.IncludeLegacyCoreContext)
        {
            options.SwaggerDoc("Core", new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "Core",
            });
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "v1",
            });
        }
    }

    private static bool ShouldIncludeInDocument(
        string swaggerDocumentName,
        string relativePath,
        string documentName,
        WorkflowConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(swaggerDocumentName, "v1", StringComparison.OrdinalIgnoreCase))
            swaggerDocumentName = "Core";

        string path = NormalizePath(relativePath);
        string rootPath = string.IsNullOrWhiteSpace(configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(swaggerDocumentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? configuration.IncludeLegacyCoreContext && MatchesContextRoute(path, "Api/Core")
            : MatchesContextRoute(path, rootPath);
    }

    private static bool MatchesContextRoute(string path, string rootPath)
    {
        string normalizedPath = NormalizePath(rootPath);
        return path.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{normalizedPath}/", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            typeof(HttpContext),
            ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());
        services.AddScoped(typeof(HttpRequest), ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60);
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}

