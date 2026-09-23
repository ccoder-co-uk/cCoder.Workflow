// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Services.Foundations;


namespace cCoder.Workflow;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "Workflow";

    public static WebApplication StartWorkflowWeb(this WebApplication app, ILogger log = null) =>
        app.UseWorkflowExposure(log: log);

    public static WebApplication StartWorkflowHostedServices(this WebApplication app)
    {
        PopulateMetadataTypeCache(app: app);
        return app;
    }

    private static WebApplication UseWorkflowExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Workflow");
        PopulateMetadataTypeCache(app: app);
        app.MapHub<WorkflowHub>(pattern: "/Api/Hubs/Workflow");
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