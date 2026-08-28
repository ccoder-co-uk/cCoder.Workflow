// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Workflow;
using Workflow.HostedServices.Exposures;
using Workflow.HostedServices.Extensions;
using Workflow.HostedServices.Models;
using Workflow.HostedServices.Services.Processings;

namespace Workflow.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(
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
        services.AddEventingHostedServices(
            configuration: appConfiguration.Eventing);

        services.AddSecurityData(configuration: appConfiguration.SecurityData);
        services.AddSecurityHostedServices(
            configuration: appConfiguration.Security);

        services.AddHttpEventingHostedServices(configure: options =>
        {
            options.HubUrl =
                appConfiguration.Eventing.Http.HubUrl;

            options.MaxConcurrency =
                appConfiguration.Eventing.Http.MaxConcurrency;
        });

        services.AddWorkflowHostedServices(
            configuration: appConfiguration.Workflow);

        return services;
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddSingleton<IHealthProcessingService, HealthProcessingService>();
        services.AddSingleton<IHealthManager, HealthProcessingService>();
        services.AddSingleton<IHomeProcessingService, HomeProcessingService>();
        services.AddSingleton<IHomeManager, HomeProcessingService>();
    }

    private static void AddExposures(
        this IServiceCollection services) =>
        services.AddControllers();
}