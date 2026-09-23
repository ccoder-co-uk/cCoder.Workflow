// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Processings;
using cCoder.Workflow.Engine.Services.Coordinations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Workflow.Engine;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngineHostedServices(
        this IServiceCollection services)
    {
        services.AddLogging();
        services.AddBrokers();
        services.AddOrchestrations();
        services.AddFoundations();

        return services;
    }

    private static void AddBrokers(
        this IServiceCollection services)
    {
        services.AddTransient<
            Brokers.Loggings.ILoggingBroker,
            Brokers.Loggings.LoggingBroker>();
        services.AddTransient<IRoslynScriptBroker, RoslynScriptBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<IReflectionBroker, ReflectionBroker>();
        services.AddTransient<
            IWorkflowHttpClientBroker,
            WorkflowHttpClientBroker>();
        services.AddTransient<
            IWorkflowHubConnectionBroker,
            WorkflowHubConnectionBroker>();
        services.AddTransient<
            IWorkflowContextBroker,
            WorkflowContextBroker>();
    }

    private static void AddOrchestrations(
        this IServiceCollection services)
    {
        services.AddTransient<IFlowRunner, FlowRunner>();
        services.AddTransient<
            IWorkflowScriptExecutionService,
            WorkflowScriptExecutionService>();
        services.AddTransient<
            IFlowExecutionOrchestrationService,
            FlowExecutionOrchestrationService>();
        services.AddTransient<
            IWorkflowLifecycleOrchestrationService,
            WorkflowLifecycleOrchestrationService>();
        services.AddTransient<
            IWorkflowRequestCoordinationService,
            WorkflowRequestCoordinationService>();
    }

    private static void AddFoundations(
        this IServiceCollection services)
    {
        services.AddTransient<
            IWorkflowScriptExecutionProcessingService,
            WorkflowScriptExecutionProcessingService>();
        services.AddTransient<IScriptService, ScriptService>();
        services.AddTransient<
            IFlowCommunicationService,
            FlowCommunicationService>();
        services.AddTransient<
            IFlowInstanceDataService,
            FlowInstanceDataService>();
        services.AddTransient<
            IWorkflowRuntimeService,
            WorkflowRuntimeService>();
        services.AddTransient<
            IFlowResultService,
            FlowResultService>();
    }
}