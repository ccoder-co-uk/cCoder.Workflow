using cCoder.Workflow.Engine.Exposures;
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
        services.AddTransient<IFlowExecutionOrchestrationService, FlowExecutionOrchestrationService>();
        services.AddTransient<IWorkflowScriptExecutionOrchestrationService, WorkflowScriptExecutionOrchestrationService>();
        services.AddTransient<IScriptProcessingService>(_ =>
            new ScriptRunner((_, _) => Task.CompletedTask));

        return services;
    }
}
