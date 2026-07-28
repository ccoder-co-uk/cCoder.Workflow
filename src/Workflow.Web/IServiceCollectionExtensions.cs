// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Security;
using cCoder.Workflow;
using Workflow.Web.Models;
using Workflow.Web.Services.Processings;

namespace Workflow.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WorkflowWebConfiguration> configure = null)
    {
        WorkflowWebConfiguration workflowWebConfiguration = new();
        configuration.Bind(instance: workflowWebConfiguration);
        configure?.Invoke(obj: workflowWebConfiguration);

        services.AddProcessings();
        services.AddExposures();
        services.AddData(configuration: workflowWebConfiguration.Data);
        services.AddEventingWeb(configuration: workflowWebConfiguration.Eventing);
        services.AddSecurityWeb(configuration: workflowWebConfiguration.Security);
        services.AddHttpEventingWeb(configure: options =>
        {
            options.HubUrl = workflowWebConfiguration.Eventing.Http.HubUrl;
            options.MaxConcurrency =
                workflowWebConfiguration.Eventing.Http.MaxConcurrency;
        });

        services.AddWorkflowWeb(
            configuration: workflowWebConfiguration.Workflow);

        return services;
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddScoped<ICoreAppProcessingService, CoreAppProcessingService>();
        services.AddScoped<ICoreUserProcessingService, CoreUserProcessingService>();
        services.AddSingleton<IHealthProcessingService, HealthProcessingService>();
    }

    private static void AddExposures(
        this IServiceCollection services) =>
        services.AddControllers();
}