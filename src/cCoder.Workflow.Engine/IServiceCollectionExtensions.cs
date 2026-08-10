// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Processings;
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
        services.AddProcessings();

        return services;
    }

    private static void AddBrokers(
        this IServiceCollection services)
    {
        services.AddTransient<Brokers.Loggings.ILoggingBroker, Brokers.Loggings.LoggingBroker>();
        services.AddTransient<RoslynScriptDependency>();
        services.AddTransient<IScriptBroker, ScriptBroker>();
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
            FlowExecutionOrchestrationAdapter>();
        services.AddTransient<
            IWorkflowScriptExecutionOrchestrationService,
            WorkflowScriptExecutionOrchestrationAdapter>();
        services.AddTransient<
            IWorkflowRequestOrchestrationService,
            WorkflowRequestOrchestrationService>();
    }

    private static void AddProcessings(
        this IServiceCollection services)
    {
        services.AddTransient<
            IWorkflowScriptExecutionProcessingService,
            WorkflowScriptExecutionProcessingService>();
        services.AddTransient<
            IFlowCommunicationProcessingService,
            FlowCommunicationProcessingService>();
        services.AddTransient<
            IFlowInstanceProcessingService,
            FlowInstanceProcessingService>();
        services.AddTransient<
            IFlowResultProcessingService,
            FlowResultProcessingService>();
    }
}