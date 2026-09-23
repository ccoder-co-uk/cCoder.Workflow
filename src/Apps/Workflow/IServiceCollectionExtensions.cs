// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Workflow.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workflow.Brokers.Http;
using Workflow.Brokers.Loggings;
using Workflow.Brokers.WorkflowExecutions;
using Workflow.Brokers.WorkflowFunctionStreams;
using Workflow.Brokers.WorkflowJson;
using Workflow.Brokers.WorkflowScriptExecutions;
using Workflow.Models;
using Workflow.Services.Foundations.WorkflowExecutions;
using Workflow.Services.Foundations.WorkflowFunctionStreams;
using Workflow.Services.Foundations.WorkflowHttpResponses;
using Workflow.Services.Foundations.WorkflowScriptExecutions;
using Workflow.Services.Orchestrations.WorkflowFunctions;
using Workflow.Services.Orchestrations.WorkflowScriptFunctions;

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

        services.AddBrokers();
        services.AddFoundations();
        services.AddOrchestrations();
        services.AddData(configuration: appConfiguration.CoreData);
        services.AddWorkflowEngineHostedServices();

        return services;
    }

    private static void AddBrokers(
        this IServiceCollection services)
    {
        services.AddTransient<IHttpResponseBroker, HttpResponseBroker>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<
            IWorkflowExecutionBroker,
            WorkflowExecutionBroker>();
        services.AddTransient<
            IWorkflowFunctionStreamBroker,
            WorkflowFunctionStreamBroker>();
        services.AddTransient<IWorkflowJsonBroker, WorkflowJsonBroker>();
        services.AddTransient<
            IWorkflowScriptExecutionBroker,
            WorkflowScriptExecutionBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<
            IWorkflowExecutionService,
            WorkflowExecutionService>();
        services.AddTransient<
            IWorkflowFunctionStreamService,
            WorkflowFunctionStreamService>();
        services.AddTransient<
            IWorkflowHttpResponseService,
            WorkflowHttpResponseService>();
        services.AddTransient<
            IWorkflowScriptExecutionService,
            WorkflowScriptExecutionService>();
    }

    private static void AddOrchestrations(
        this IServiceCollection services)
    {
        services.AddTransient<
            IWorkflowFunctionsOrchestrationService,
            WorkflowFunctionsOrchestrationService>();
        services.AddTransient<
            IWorkflowScriptFunctionsOrchestrationService,
            WorkflowScriptFunctionsOrchestrationService>();
    }
}