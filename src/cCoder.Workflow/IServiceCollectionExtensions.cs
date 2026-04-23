using cCoder.Data;
using cCoder.Data.Brokers;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Exposures.HostedServices;
using cCoder.Workflow.Services.Aggregations;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Workflow.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;
using IUserBroker = cCoder.Workflow.Brokers.IUserBroker;
using JsonBroker = cCoder.Workflow.Brokers.JsonBroker;
using UserBroker = cCoder.Workflow.Brokers.UserBroker;


namespace cCoder.Workflow;

public static class IServiceCollectionExtensions
{
    public static void AddWorkflow(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddEventHandlers();
    }

    public static void AddWorkflowApi(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddWorkflow();
        services.AddApi("Workflow", ConfigureWorkflowApiModel, builder);
    }

    public static void AddWorkflowHostedServices(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();
        services.AddTransient<IWorkflowInstanceManagementOrchestrationService, WorkflowInstanceManagementOrchestrationService>();
        services.AddHostedService<WorkflowInstanceManagementHostedService>();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<FlowDefinition>();
        services.AddEventingForType<FlowInstanceData>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<(int, Package)>();
        services.AddEventingForType<ScheduledTask>();
        services.AddEventingForType<WorkflowEvent>();
        services.AddEventingForType<cCoder.Data.Models.Workflow.FlowDefinition>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IFlowDefinitionEventBroker, FlowDefinitionEventBroker>();
        services.AddTransient<IFlowInstanceDataEventBroker, FlowInstanceDataEventBroker>();
        services.AddTransient<IWorkflowEventEventBroker, WorkflowEventEventBroker>();
        services.AddTransient<IFlowDefinitionBroker, FlowDefinitionBroker>();
        services.AddTransient<IFlowInstanceDataBroker, FlowInstanceDataBroker>();
        services.AddTransient<IWorkflowInstanceManagementBroker, WorkflowInstanceManagementBroker>();
        services.AddTransient<IWorkflowEventBroker, WorkflowEventBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<IUserBroker, UserBroker>();
    }

    private static void AddCoordinations(this IServiceCollection services) =>
        services.AddTransient<IFlowDefinitionCoordinationService, FlowDefinitionCoordinationService>();

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IWorkflowAppExposure, WorkflowAppExposure>();
        services.AddTransient<IWorkflowPackageManager, WorkflowPackageManager>();
        services.AddTransient<IWorkflowEventHandlers, WorkflowEventHandlers>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IFlowDefinitionService, FlowDefinitionService>();
        services.AddTransient<IFlowInstanceDataService, FlowInstanceDataService>();
        services.AddTransient<IWorkflowMetadataTypeService, WorkflowMetadataTypeService>();
        services.AddTransient<IWorkflowEventService, WorkflowEventService>();
        services.AddTransient<IFlowDefinitionEventService, FlowDefinitionEventService>();
        services.AddTransient<IFlowInstanceDataEventService, FlowInstanceDataEventService>();
        services.AddTransient<IWorkflowEventEventService, WorkflowEventEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IEventHandlingOrchestrationService, EventHandlingOrchestrationService>();
        services.AddTransient<IWorkflowMigrationAggregationService, WorkflowMigrationAggregationService>();
        services.AddTransient<IFlowDefinitionOrchestrationService, FlowDefinitionOrchestrationService>();
        services.AddTransient<IFlowInstanceDataOrchestrationService, FlowInstanceDataOrchestrationService>();
        services.AddTransient<IWorkflowInstanceManagementOrchestrationService, WorkflowInstanceManagementOrchestrationService>();
        services.AddTransient<IWorkflowEventOrchestrationService, WorkflowEventOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IFlowDefinitionEventProcessingService, FlowDefinitionEventProcessingService>();
        services.AddTransient<IFlowDefinitionProcessingService, FlowDefinitionProcessingService>();
        services.AddTransient<IFlowInstanceDataEventProcessingService, FlowInstanceDataEventProcessingService>();
        services.AddTransient<IFlowInstanceDataProcessingService, FlowInstanceDataProcessingService>();
        services.AddTransient<IWorkflowEventEventProcessingService, WorkflowEventEventProcessingService>();
        services.AddTransient<IWorkflowEventProcessingService, WorkflowEventProcessingService>();
    }

    private static void ConfigureWorkflowApiModel(ODataConventionModelBuilder builder) =>
        new WorkflowModelBuilder(builder).Configure();

    private static void AddApi(
        this IServiceCollection services,
        string routePrefix,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, routePrefix, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();

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
                .AddRouteComponents($"Api/{routePrefix}", routeModel, batchHandler);

            if (builder is null)
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string routePrefix,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, routePrefix);
            options.DocInclusionPredicate(
                (documentName, apiDescription) =>
                    ShouldIncludeInDocument(documentName, apiDescription.RelativePath, routePrefix));

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
        string routePrefix)
    {
        options.SwaggerDoc(routePrefix, new OpenApiInfo
        {
            Title = $"{routePrefix} API definition",
            Version = routePrefix,
        });
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

    private static bool ShouldIncludeInDocument(
        string documentName,
        string relativePath,
        string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(documentName, "v1", StringComparison.OrdinalIgnoreCase))
            documentName = "Core";

        string path = NormalizePath(relativePath);

        return string.Equals(documentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? MatchesContextRoute(path, "Core")
            : MatchesContextRoute(path, routePrefix);
    }

    private static bool MatchesContextRoute(string path, string context)
    {
        string prefix = $"/Api/{context}";
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
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










