// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Exposures.Hubs;
using cCoder.Workflow.Services.Foundations;


namespace cCoder.Workflow;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "Workflow";
    private static readonly ConditionalWeakTable<WebApplication, object> StartedHostedServiceApps = new();
    private static readonly object StartedHostedServiceAppsLock = new();

    public static WebApplication StartWorkflowWeb(this WebApplication app, ILogger log = null) =>
        app.UseWorkflowExposure(log: log);

    public static WebApplication StartWorkflowHostedServices(this WebApplication app)
    {
        if (!TryMarkWorkflowHostedServicesStarted(app: app))
        {
            return app;
        }

        return app.UseWorkflowEventHandlers()
            .UseWorkflowScheduledTaskExecutionHandlers()
            .UseWorkflowQueuedInstanceExecutionHandlers();
    }

    private static bool TryMarkWorkflowHostedServicesStarted(WebApplication app)
    {
        lock (StartedHostedServiceAppsLock)
        {
            if (StartedHostedServiceApps.TryGetValue(key: app, value: out _))
            {
                return false;
            }

            StartedHostedServiceApps.Add(key: app, value: new object());
            return true;
        }
    }

    private static WebApplication UseWorkflowExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Workflow");
        PopulateMetadataTypeCache(app: app);
        app.MapHub<WorkflowHub>(pattern: "/Api/Hubs/Workflow");
        return app;
    }

    private static WebApplication UseWorkflowEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger(categoryName: "WorkflowStartup");

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
        {
            try
            {
                handlers.ListenToAllEvents();
            }
            catch (Exception ex)
            {
                logger.LogWarning(
exception: ex,
message: "Workflow event handler registration was skipped because the event hub is unavailable in the current host.");
            }
        }

        return app;
    }

    private static WebApplication UseWorkflowScheduledTaskExecutionHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
        {
            handlers.ListenToScheduledTaskExecuteEvents();
        }

        return app;
    }

    private static WebApplication UseWorkflowQueuedInstanceExecutionHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
        {
            handlers.ListenToQueuedFlowInstanceExecuteEvents();
        }

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(scope: MetadataScope))
        {
            metadataTypeCache.Set(
scope: MetadataScope,
typeSetPayloads: new[]
                {
                    app.Services.GetRequiredService<IWorkflowMetadataTypeService>()
                .GetCoreMetadata(),
                    app.Services.GetRequiredService<IWorkflowMetadataTypeService>()
                .GetSharedMetadata(),
                }.Select(selector: static metadata => JsonSerializer.Serialize(metadata))
            );
        }
    }
}