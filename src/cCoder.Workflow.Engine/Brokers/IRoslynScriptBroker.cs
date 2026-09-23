// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

namespace cCoder.Workflow.Engine.Brokers;

internal interface IRoslynScriptBroker
{
    Assembly[] GetCurrentAssemblies();

    Assembly GetExecutingAssembly();

    string[] GetFiles(string path, string searchPattern);

    Assembly LoadFile(string path);

    Type[] GetExportedTypes(Assembly assembly);

    ScriptOptions BuildOptions(
        IEnumerable<Assembly> references,
        string[] imports);

    Task<T> EvaluateAsync<T>(
        string code,
        ScriptOptions options,
        object globals = null,
        Type globalsType = null);
}