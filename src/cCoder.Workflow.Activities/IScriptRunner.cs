// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities;

public interface IScriptRunner
{
    Task Run(string code, string[] imports, object args, Action<WorkflowLogLevel, string> log);
    Task<T> Run<T>(string code, string[] imports, object args = null, Action<WorkflowLogLevel, string> log = null);
    Task<T> BuildScript<T>(string code, string[] imports, Action<WorkflowLogLevel, string> log);
}