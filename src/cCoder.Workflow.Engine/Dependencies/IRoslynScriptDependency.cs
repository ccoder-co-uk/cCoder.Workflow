// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Dependencies;

internal interface IRoslynScriptDependency
{
    Task<T> BuildScriptAsync<T>(
        string code,
        string[] imports,
        Action<WorkflowLogLevel, string> log);

    Task<T> RunScriptAsync<T>(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log);
}