// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Security;
using cCoder.Workflow;
using Workflow.HostedServices.Exposures;
using Workflow.HostedServices.Extensions;
using Workflow.HostedServices.Models;
using Workflow.HostedServices.Services.Processings;

namespace Workflow.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WorkflowHostedServicesConfiguration> configure = null)
    {
        WorkflowHostedServicesConfiguration hostedServicesConfiguration =
            configuration.CreateWorkflowHostedServicesConfiguration();

        configuration.Bind(instance: hostedServicesConfiguration);
        configure?.Invoke(obj: hostedServicesConfiguration);

        services.AddProcessings();
        services.AddExposures();
        services.AddData(configuration: hostedServicesConfiguration.Data);
        services.AddEventingHostedServices(
            configuration: hostedServicesConfiguration.Eventing);

        services.AddSecurityHostedServices(
            configuration: hostedServicesConfiguration.Security);

        services.AddHttpEventingHostedServices(configure: options =>
        {
            options.HubUrl =
                hostedServicesConfiguration.Eventing.Http.HubUrl;

            options.MaxConcurrency =
                hostedServicesConfiguration.Eventing.Http.MaxConcurrency;
        });

        services.AddWorkflowHostedServices(
            configuration: hostedServicesConfiguration.Workflow);

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
