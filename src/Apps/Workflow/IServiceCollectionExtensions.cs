// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Workflow.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workflow.Brokers.Http;
using Workflow.Brokers.Loggings;
using Workflow.Models;
using Workflow.Services.Foundations.WorkflowFunctions;

namespace Workflow;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflow(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration appConfiguration = new() { CoreData = new() };
        configuration.Bind(instance: appConfiguration);
        configure?.Invoke(obj: appConfiguration);

        services.AddProcessings();
        services.AddTransient<IHttpResponseBroker, HttpResponseBroker>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddData(configuration: appConfiguration.CoreData);
        services.AddWorkflowEngineHostedServices();

        return services;
    }

    private static void AddProcessings(
        this IServiceCollection services)
    {
        services.AddTransient<
            IWorkflowFunctionsService,
            WorkflowFunctionsService>();

    }
}