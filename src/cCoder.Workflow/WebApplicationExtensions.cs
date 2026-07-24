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
        app.UseWorkflowExposure(log);

    public static WebApplication StartWorkflowHostedServices(this WebApplication app)
    {
        if (!TryMarkWorkflowHostedServicesStarted(app))
            return app;

        return app.UseWorkflowEventHandlers()
            .UseWorkflowScheduledTaskExecutionHandlers()
            .UseWorkflowQueuedInstanceExecutionHandlers();
    }

    private static bool TryMarkWorkflowHostedServicesStarted(WebApplication app)
    {
        lock (StartedHostedServiceAppsLock)
        {
            if (StartedHostedServiceApps.TryGetValue(app, out _))
                return false;

            StartedHostedServiceApps.Add(app, new object());
            return true;
        }
    }

    private static WebApplication UseWorkflowExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation("Initialising Workflow");
        PopulateMetadataTypeCache(app);
        app.MapHub<WorkflowHub>("/Api/Hubs/Workflow");
        return app;
    }

    private static WebApplication UseWorkflowEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("WorkflowStartup");

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
        {
            try
            {
                handlers.ListenToAllEvents();
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Workflow event handler registration was skipped because the event hub is unavailable in the current host.");
            }
        }

        return app;
    }

    private static WebApplication UseWorkflowScheduledTaskExecutionHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
            handlers.ListenToScheduledTaskExecuteEvents();

        return app;
    }

    private static WebApplication UseWorkflowQueuedInstanceExecutionHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IWorkflowEventHandlers handlers in services.GetServices<IWorkflowEventHandlers>())
            handlers.ListenToQueuedFlowInstanceExecuteEvents();

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(MetadataScope))
        {
            metadataTypeCache.Set(
                MetadataScope,
                new[]
                {
                    app.Services.GetRequiredService<IWorkflowMetadataTypeService>().GetCoreMetadata(),
                    app.Services.GetRequiredService<IWorkflowMetadataTypeService>().GetSharedMetadata(),
                }.Select(static metadata => JsonSerializer.Serialize(metadata))
            );
        }
    }
}