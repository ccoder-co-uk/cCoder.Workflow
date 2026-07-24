// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Services.Orchestrations;
using cCoder.Workflow.Engine.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Workflow.Engine;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngine(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddTransient<IFlowRunner, FlowRunner>();
        services.AddTransient<IWorkflowScriptExecutionService, WorkflowScriptExecutionService>();
        services.AddTransient<
            IFlowExecutionOrchestrationService,
            FlowExecutionOrchestrationAdapter>();
        services.AddTransient<
            IWorkflowScriptExecutionOrchestrationService,
            WorkflowScriptExecutionOrchestrationAdapter>();
        services.AddTransient<
            IWorkflowRequestOrchestrationService,
            WorkflowRequestOrchestrationService>();
        services.AddTransient<IRoslynScriptDependency, RoslynScriptDependency>();
        services.AddTransient<
            IWorkflowContextBroker,
            WorkflowContextBroker>();
        services.AddTransient<ScriptBroker>();
        services.AddTransient<IScriptBroker>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<ScriptBroker>());
        services.AddTransient<IScriptProcessingService>(
            implementationFactory: serviceProvider =>
                serviceProvider.GetRequiredService<ScriptBroker>());
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

        return services;
    }
}