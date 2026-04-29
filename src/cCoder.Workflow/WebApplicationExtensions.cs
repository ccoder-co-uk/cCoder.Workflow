using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Exposures.Hubs;
using cCoder.Workflow.Services.Foundations;


namespace cCoder.Workflow;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "Workflow";

    public static WebApplication StartWorkflowWeb(this WebApplication app, ILogger log = null) =>
        app.UseWorkflowExposure(log)
            .UseWorkflowEventHandlers();

    public static WebApplication StartWorkflowHostedServices(this WebApplication app) =>
        app.UseWorkflowEventHandlers()
            .UseWorkflowScheduledTaskExecutionHandlers();

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





