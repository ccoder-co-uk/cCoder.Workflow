// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Workflow;
using Workflow.Web.Exposures;
using Workflow.Web.Extensions;
using Workflow.Web.Models;
using Workflow.Web.Services.Processings;

namespace Workflow.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration appConfiguration =
            configuration.CreateAppConfiguration();

        configuration.Bind(instance: appConfiguration);
        configure?.Invoke(obj: appConfiguration);

        services.AddProcessings();
        services.AddExposures();
        services.AddData(configuration: appConfiguration.CoreData);
        services.AddEventingWeb(configuration: appConfiguration.Eventing);
        services.AddSecurityData(configuration: appConfiguration.SecurityData);
        services.AddSecurityWeb(configuration: appConfiguration.Security);
        services.AddHttpEventingWeb(configure: options =>
        {
            options.HubUrl = appConfiguration.Eventing.Http.HubUrl;
            options.MaxConcurrency =
                appConfiguration.Eventing.Http.MaxConcurrency;
        });

        services.AddWorkflowWeb(
            configuration: appConfiguration.Workflow);

        return services;
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddScoped<ICoreAppProcessingService, CoreAppProcessingService>();
        services.AddScoped<ICoreAppManager, CoreAppProcessingService>();
        services.AddScoped<ICoreUserProcessingService, CoreUserProcessingService>();
        services.AddScoped<ICoreUserManager, CoreUserProcessingService>();
        services.AddSingleton<IHealthProcessingService, HealthProcessingService>();
        services.AddSingleton<IHealthManager, HealthProcessingService>();
    }

    private static void AddExposures(
        this IServiceCollection services) =>
        services.AddControllers();
}