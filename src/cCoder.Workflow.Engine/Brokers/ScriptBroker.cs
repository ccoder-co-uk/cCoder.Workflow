// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Services.Processings;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class ScriptBroker(
    IRoslynScriptDependency roslynScriptDependency)
    : IScriptBroker,
      IScriptProcessingService
{
    public Task<T> BuildScript<T>(
        string code,
        string[] imports,
        Action<WorkflowLogLevel, string> log) =>
        roslynScriptDependency.BuildScriptAsync<T>(
            code: code,
            imports: imports,
            log: log);

    public Task<T> Run<T>(
        string code,
        string[] imports,
        object args = null,
        Action<WorkflowLogLevel, string> log = null) =>
        roslynScriptDependency.RunScriptAsync<T>(
            code: code,
            imports: imports,
            args: args,
            log: log);

    public Task Run(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log) =>
        roslynScriptDependency.RunScriptAsync<bool>(
            code: $"{code};return true;",
            imports: imports,
            args: args,
            log: log);
}